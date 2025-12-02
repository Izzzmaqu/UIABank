using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Cuentas;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace UIABank.BW.CU
{
    public class HistorialService : IHistorialService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IPagoServicioRepository _pagoServicioRepository;
        private readonly ITransferenciaDA _transferenciaDA;
        private readonly ICuentaRepository _cuentaRepository;

        public HistorialService(
            IClienteRepository clienteRepository,
            IPagoServicioRepository pagoServicioRepository,
            ITransferenciaDA transferenciaDA,
            ICuentaRepository cuentaRepository)
        {
            _clienteRepository = clienteRepository;
            _pagoServicioRepository = pagoServicioRepository;
            _transferenciaDA = transferenciaDA;
            _cuentaRepository = cuentaRepository;
        }

        // RF-F1: Historial cliente

        public async Task<List<MovimientoHistorial>> ObtenerHistorialClienteAsync(
            int clienteId,
            DateTime? desde,
            DateTime? hasta,
            TipoMovimiento? tipo,
            string? estado)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(clienteId);
            if (cliente == null)
                throw new ArgumentException("Cliente no encontrado.", nameof(clienteId));

            if (cliente.Usuario == null)
                throw new InvalidOperationException("El cliente no está asociado a un usuario.");

            var usuarioId = cliente.Usuario.Id;

            var movimientos = new List<MovimientoHistorial>();

            // Pagos de servicios 
            if (!tipo.HasValue || tipo.Value == TipoMovimiento.PagoServicio)
            {
                var pagos = await _pagoServicioRepository.ObtenerPorClienteAsync(
                    clienteId,
                    desde,
                    hasta,
                    soloProgramados: false);

                if (!string.IsNullOrWhiteSpace(estado) &&
                    Enum.TryParse<EstadoPagoServicio>(estado, true, out var estadoPago))
                {
                    pagos = pagos
                        .Where(p => p.Estado == estadoPago)
                        .ToList();
                }

                movimientos.AddRange(
                    pagos.Select(p => new MovimientoHistorial
                    {
                        Fecha = p.FechaCreacion,
                        Tipo = TipoMovimiento.PagoServicio,
                        Descripcion = $"{p.ProveedorServicio} - Contrato {p.NumeroContrato}",
                        Monto = p.Monto,
                        Comision = 0m,
                        Moneda = p.Moneda,
                        Estado = p.Estado.ToString(),
                        Referencia = $"PAGO-{p.Id:D8}",
                        CuentaId = null 
                    }));
            }

            // Transferencias
            if (!tipo.HasValue || tipo.Value == TipoMovimiento.Transferencia)
            {
                var transferencias = await _transferenciaDA.ListarPorUsuarioAsync(usuarioId);

                var query = transferencias.AsQueryable();

                if (desde.HasValue)
                    query = query.Where(t => t.FechaCreacion.Date >= desde.Value.Date);

                if (hasta.HasValue)
                    query = query.Where(t => t.FechaCreacion.Date <= hasta.Value.Date);

                if (!string.IsNullOrWhiteSpace(estado) &&
                    Enum.TryParse<EstadoTransferencia>(estado, true, out var estadoTransfer))
                {
                    query = query.Where(t => t.Estado == estadoTransfer);
                }

                movimientos.AddRange(
                    query.Select(t => MapTransferenciaToMovimiento(t)));
            }

            return movimientos
                .OrderByDescending(m => m.Fecha)
                .ToList();
        }

        //RF-F1: Historial admin/gestor

        public async Task<List<MovimientoHistorial>> ObtenerHistorialPorClienteCuentaAsync(
            int clienteId,
            Guid? cuentaId,
            DateTime? desde,
            DateTime? hasta,
            TipoMovimiento? tipo,
            string? estado)
        {
            
            var movimientos = await ObtenerHistorialClienteAsync(
                clienteId,
                desde,
                hasta,
                tipo,
                estado);

            if (!cuentaId.HasValue)
                return movimientos;

            
            var cuenta = await _cuentaRepository.ObtenerPorIdAsync(cuentaId.Value);
            if (cuenta == null)
                throw new ArgumentException("La cuenta especificada no existe.", nameof(cuentaId));

            var numeroCuenta = cuenta.Numero;

            return movimientos
                .Where(m =>
                    
                    (m.CuentaId.HasValue && m.CuentaId.Value == cuentaId.Value)
                    ||
                  
                    (m.Tipo == TipoMovimiento.PagoServicio &&
                     m.Descripcion.Contains(numeroCuenta, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(m => m.Fecha)
                .ToList();
        }

        //RF-F2: Extractos mensuales

        public async Task<ExtractoMensual> GenerarExtractoMensualAsync(
            int clienteId,
            Guid cuentaId,
            int anio,
            int mes)
        {
            if (mes < 1 || mes > 12)
                throw new ArgumentOutOfRangeException(nameof(mes), "El mes debe estar entre 1 y 12.");

            var cuenta = await _cuentaRepository.ObtenerPorIdAsync(cuentaId);
            if (cuenta == null)
                throw new ArgumentException("La cuenta no existe.", nameof(cuentaId));

            var inicioMes = new DateTime(anio, mes, 1);
            var finMes = inicioMes.AddMonths(1).AddTicks(-1);

          
            var movimientosCuenta = await ObtenerHistorialPorClienteCuentaAsync(
                clienteId,
                cuentaId,
                null,  
                null,
                null,
                null);

           
            var antesMes = movimientosCuenta.Where(m => m.Fecha < inicioMes).ToList();
            var enMes = movimientosCuenta
                .Where(m => m.Fecha >= inicioMes && m.Fecha <= finMes)
                .OrderBy(m => m.Fecha)
                .ToList();
            var despuesMes = movimientosCuenta.Where(m => m.Fecha > finMes).ToList();

            decimal Impacto(MovimientoHistorial m)
            {
                
                return -1m * (m.Monto + m.Comision);
            }

            var saldoActual = cuenta.Saldo;

            var impactoDespuesMes = despuesMes.Sum(Impacto);
            var saldoFinMes = saldoActual - impactoDespuesMes;

            var impactoMes = enMes.Sum(Impacto);
            var saldoInicial = saldoFinMes - impactoMes;

            var totalDebitos = enMes.Sum(m => m.Monto + m.Comision);
            var totalCreditos = 0m; 
            var totalComisiones = enMes.Sum(m => m.Comision);

            return new ExtractoMensual
            {
                ClienteId = clienteId,
                CuentaId = cuentaId,
                NumeroCuenta = cuenta.Numero,
                Anio = anio,
                Mes = mes,
                SaldoInicial = saldoInicial,
                SaldoFinal = saldoFinMes,
                TotalDebitos = totalDebitos,
                TotalCreditos = totalCreditos,
                TotalComisiones = totalComisiones,
                Movimientos = enMes
            };
        }

        public async Task<byte[]> GenerarExtractoMensualCsvAsync(
            int clienteId,
            Guid cuentaId,
            int anio,
            int mes)
        {
            var extracto = await GenerarExtractoMensualAsync(clienteId, cuentaId, anio, mes);

            var sb = new StringBuilder();
            sb.AppendLine("Extracto mensual");
            sb.AppendLine($"ClienteId;{extracto.ClienteId}");
            sb.AppendLine($"Cuenta;{extracto.NumeroCuenta}");
            sb.AppendLine($"Periodo;{extracto.Anio}-{extracto.Mes:D2}");
            sb.AppendLine($"SaldoInicial;{extracto.SaldoInicial}");
            sb.AppendLine($"SaldoFinal;{extracto.SaldoFinal}");
            sb.AppendLine($"TotalDebitos;{extracto.TotalDebitos}");
            sb.AppendLine($"TotalCreditos;{extracto.TotalCreditos}");
            sb.AppendLine($"TotalComisiones;{extracto.TotalComisiones}");
            sb.AppendLine();
            sb.AppendLine("Fecha;Tipo;Descripcion;Monto;Comision;Moneda;Estado;Referencia");

            foreach (var m in extracto.Movimientos)
            {
                sb.AppendLine($"{m.Fecha:yyyy-MM-dd HH:mm};{m.Tipo};{m.Descripcion};{m.Monto};{m.Comision};{m.Moneda};{m.Estado};{m.Referencia}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> GenerarExtractoMensualPdfAsync(
    int clienteId,
    Guid cuentaId,
    int anio,
    int mes)
        {
            var extracto = await GenerarExtractoMensualAsync(clienteId, cuentaId, anio, mes);

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text(text =>
                        {
                            text.Span("UIABank").FontSize(20).SemiBold();
                            text.Line("");
                            text.Span("Extracto Mensual").FontSize(16);
                        });

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Cliente Id: {extracto.ClienteId}");
                        col.Item().Text($"Cuenta: {extracto.NumeroCuenta}");
                        col.Item().Text($"Periodo: {extracto.Anio}-{extracto.Mes:D2}");

                        col.Item().LineHorizontal(1);

                        col.Item().Text($"Saldo inicial: {extracto.SaldoInicial:C}");
                        col.Item().Text($"Saldo final:   {extracto.SaldoFinal:C}");
                        col.Item().Text($"Total débitos: {extracto.TotalDebitos:C}");
                        col.Item().Text($"Total comisiones: {extracto.TotalComisiones:C}");

                        col.Item().LineHorizontal(1);

                        col.Item().Text("Movimientos").FontSize(13).SemiBold();

                      
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(90);   
                                columns.RelativeColumn(2);   
                                columns.ConstantColumn(70);  
                                columns.ConstantColumn(70);  
                                columns.ConstantColumn(70);  
                                columns.ConstantColumn(60);   
                                columns.ConstantColumn(70);   
                            });

                           
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Fecha");
                                header.Cell().Element(HeaderCell).Text("Descripción");
                                header.Cell().Element(HeaderCell).Text("Tipo");
                                header.Cell().Element(HeaderCell).Text("Monto");
                                header.Cell().Element(HeaderCell).Text("Comisión");
                                header.Cell().Element(HeaderCell).Text("Moneda");
                                header.Cell().Element(HeaderCell).Text("Estado");

                                static IContainer HeaderCell(IContainer container) =>
                                    container
                                        .PaddingVertical(5)
                                        .DefaultTextStyle(x => x.SemiBold())
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Medium);
                            });

                            foreach (var m in extracto.Movimientos)
                            {
                                table.Cell().Element(Cell).Text(m.Fecha.ToString("yyyy-MM-dd HH:mm"));
                                table.Cell().Element(Cell).Text(m.Descripcion);
                                table.Cell().Element(Cell).Text(m.Tipo.ToString());
                                table.Cell().Element(Cell).Text(m.Monto.ToString("N2"));
                                table.Cell().Element(Cell).Text(m.Comision.ToString("N2"));
                                table.Cell().Element(Cell).Text(m.Moneda);
                                table.Cell().Element(Cell).Text(m.Estado);

                                static IContainer Cell(IContainer container) =>
                                    container
                                        .PaddingVertical(3)
                                        .BorderBottom(0.5f)
                                        .BorderColor(Colors.Grey.Lighten2);
                            }
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generado por UIABank - ").FontSize(9);
                            x.Span($"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(9);
                        });
                });
            }).GeneratePdf();   

            return pdfBytes;
        }

        //RF-F3: Comprobantes

        public async Task<byte[]> GenerarComprobanteTransferenciaPdfAsync(int transferenciaId, int clienteId)
        {
            var transferencia = await _transferenciaDA.ObtenerPorIdAsync(transferenciaId);
            if (transferencia == null)
                throw new ArgumentException("La transferencia no existe.", nameof(transferenciaId));

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A5);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(t => t.FontSize(11));

                    page.Header()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("UIABank").FontSize(18).SemiBold();
                            text.Line("");
                            text.Span("Comprobante de Transferencia").FontSize(14);
                        });

                    page.Content().Column(col =>
                    {
                        col.Spacing(6);

                        col.Item().Text($"Referencia: {transferencia.Referencia}");
                        col.Item().Text($"Fecha: {transferencia.FechaCreacion:yyyy-MM-dd HH:mm}");
                        col.Item().Text($"Monto: {transferencia.Monto:N2} {transferencia.Moneda}");
                        col.Item().Text($"Comisión: {transferencia.Comision:N2}");
                        col.Item().Text($"Estado: {transferencia.Estado}");

                        col.Item().LineHorizontal(1);

                        col.Item().Text($"Cuenta origen Id: {transferencia.CuentaOrigenId}");
                        col.Item().Text($"Cuenta destino Id: {transferencia.CuentaDestinoId}");
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Gracias por usar UIABank").FontSize(9);
                });
            }).GeneratePdf();

            return pdfBytes;
        }




        private static MovimientoHistorial MapTransferenciaToMovimiento(Transferencia t)
        {
            return new MovimientoHistorial
            {
                Fecha = t.FechaCreacion,
                Tipo = TipoMovimiento.Transferencia,
                Descripcion = $"Transferencia a cuenta destino {t.CuentaDestinoId}",
                Monto = t.Monto,
                Comision = t.Comision,
                Moneda = t.Moneda,
                Estado = t.Estado.ToString(),
                Referencia = t.Referencia,
                CuentaId = t.CuentaOrigenId
            };
        }

        public async Task<byte[]> GenerarComprobantePagoServicioPdfAsync(int pagoServicioId, int clienteId)
        {
            var pago = await _pagoServicioRepository.ObtenerPorIdAsync(pagoServicioId);
            if (pago == null)
                throw new ArgumentException("El pago de servicio no existe.", nameof(pagoServicioId));

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A5);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(t => t.FontSize(11));

                    page.Header()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("UIABank").FontSize(18).SemiBold();
                            text.Line("");
                            text.Span("Comprobante de Pago de Servicio").FontSize(14);
                        });

                    page.Content().Column(col =>
                    {
                        col.Spacing(6);

                        col.Item().Text($"Id Pago: {pago.Id}");
                        col.Item().Text($"Servicio: {pago.ProveedorServicio}");
                        col.Item().Text($"Número de contrato: {pago.NumeroContrato}");
                        col.Item().Text($"Fecha: {(pago.FechaEjecucion ?? pago.FechaCreacion):yyyy-MM-dd HH:mm}");
                        col.Item().Text($"Monto: {pago.Monto:N2} {pago.Moneda}");
                        col.Item().Text($"Cuenta origen: {pago.CuentaOrigen}");
                        col.Item().Text($"Estado: {pago.Estado}");
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Gracias por usar UIABank").FontSize(9);
                });
            }).GeneratePdf();

            return pdfBytes;
        }

    }
}

