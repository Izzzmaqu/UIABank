using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UIABank.BC.Cuentas;
using UIABank.BC.Modelos;
using UIABank.BC.Modelos.Reportes;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;

namespace UIABank.BW.CU
{
    public class ReportesService : IReportesService
    {
        private readonly ITransferenciaDA _transferenciasDA;
        private readonly IPagoServicioRepository _pagosDA;
        private readonly IClienteRepository _clientesDA;
        private readonly ICuentaRepository _cuentasDA;

        public ReportesService(
            ITransferenciaDA transferenciasDA,
            IPagoServicioRepository pagosDA,
            IClienteRepository clientesDA,
            ICuentaRepository cuentasDA)
        {
            _transferenciasDA = transferenciasDA;
            _pagosDA = pagosDA;
            _clientesDA = clientesDA;
            _cuentasDA = cuentasDA;
        }

        // ========== RF-G1: Totales por período ==========
        public async Task<ReporteTotalesDto> ObtenerTotalesAsync(DateTime desde, DateTime hasta)
        {
            var transferencias = await _transferenciasDA.ListarPorRangoFechasAsync(desde, hasta);
            var pagos = await _pagosDA.ListarPorRangoFechasAsync(desde, hasta);

            // filtrar solo exitosas
            transferencias = transferencias
                .Where(t => t.Estado == EstadoTransferencia.Exitosa)
                .ToList();

            pagos = pagos
                .Where(p => p.Estado == EstadoPagoServicio.Exitoso)
                .ToList();

            return new ReporteTotalesDto
            {
                TotalTransferencias = transferencias.Count,
                MontoTransferencias = transferencias.Sum(t => t.Monto),

                TotalPagos = pagos.Count,
                MontoPagos = pagos.Sum(p => p.Monto),

                TotalComisiones = transferencias.Sum(t => t.Comision) +
                                  pagos.Sum(p => p.Comision)
            };
        }

        // ========== RF-G1: Top 10 clientes por volumen ==========
        public async Task<List<ClienteVolumenDto>> ObtenerTop10ClientesAsync(DateTime desde, DateTime hasta)
        {
            var transferencias = await _transferenciasDA.ListarPorRangoFechasAsync(desde, hasta);
            var pagos = await _pagosDA.ListarPorRangoFechasAsync(desde, hasta);

            transferencias = transferencias
                .Where(t => t.Estado == EstadoTransferencia.Exitosa)
                .ToList();

            pagos = pagos
                .Where(p => p.Estado == EstadoPagoServicio.Exitoso)
                .ToList();

            // agrupar por cliente/usuario que ejecuta la operación
            var volumenTransferencias = transferencias
                .GroupBy(t => t.UsuarioEjecutorId)
                .Select(g => new { ClienteId = g.Key, Volumen = g.Sum(x => x.Monto) });

            var volumenPagos = pagos
                .GroupBy(p => p.ClienteId)
                .Select(g => new { ClienteId = g.Key, Volumen = g.Sum(x => x.Monto) });

            var volumenTotal = volumenTransferencias
                .Concat(volumenPagos)
                .GroupBy(x => x.ClienteId)
                .Select(g => new { ClienteId = g.Key, Volumen = g.Sum(x => x.Volumen) })
                .OrderByDescending(x => x.Volumen)
                .Take(10)
                .ToList();

            var idsClientes = volumenTotal.Select(v => v.ClienteId).ToList();
            var clientes = await _clientesDA.ObtenerPorIdsAsync(idsClientes);

            var resultado = new List<ClienteVolumenDto>();

            foreach (var v in volumenTotal)
            {
                var cliente = clientes.First(c => c.Id == v.ClienteId);
                var nombre = $"{cliente.NombreCompleto}".Trim();

                resultado.Add(new ClienteVolumenDto
                {
                    ClienteId = v.ClienteId,
                    NombreCompleto = nombre,
                    VolumenTotal = v.Volumen
                });
            }

            return resultado;
        }

        // ========== RF-G1: Volumen diario ==========
        public async Task<List<VolumenDiarioDto>> ObtenerVolumenDiarioAsync(DateTime desde, DateTime hasta)
        {
            var transferencias = await _transferenciasDA.ListarPorRangoFechasAsync(desde, hasta);
            var pagos = await _pagosDA.ListarPorRangoFechasAsync(desde, hasta);

            transferencias = transferencias
                .Where(t => t.Estado == EstadoTransferencia.Exitosa)
                .ToList();

            pagos = pagos
                .Where(p => p.Estado == EstadoPagoServicio.Exitoso)
                .ToList();

            var diarioTransfer = transferencias
                .GroupBy(t => t.FechaCreacion.Date)
                .Select(g => new { Fecha = g.Key, Cantidad = g.Count(), Monto = g.Sum(x => x.Monto) });

            // FechaEjecucion es DateTime? => hay que usar HasValue y luego .Value.Date
            var diarioPagos = pagos
                .Where(p => p.FechaEjecucion.HasValue)
                .GroupBy(p => p.FechaEjecucion!.Value.Date)
                .Select(g => new { Fecha = g.Key, Cantidad = g.Count(), Monto = g.Sum(x => x.Monto) });

            var combinado = diarioTransfer
                .Concat(diarioPagos)
                .GroupBy(x => x.Fecha)
                .Select(g => new VolumenDiarioDto
                {
                    Fecha = g.Key,
                    CantidadTransacciones = g.Sum(x => x.Cantidad),
                    MontoTotal = g.Sum(x => x.Monto)
                })
                .OrderBy(x => x.Fecha)
                .ToList();

            return combinado;
        }

        // ========== RF-G1: PDF ==========
        public async Task<byte[]> GenerarReportePdfAsync(DateTime desde, DateTime hasta)
        {
            var totales = await ObtenerTotalesAsync(desde, hasta);
            var top10 = await ObtenerTop10ClientesAsync(desde, hasta);
            var volumenDiario = await ObtenerVolumenDiarioAsync(desde, hasta);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Text($"Reporte de Operaciones {desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy}")
                        .SemiBold().FontSize(16).AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Item().Text("Totales de operaciones").SemiBold().FontSize(12);
                        col.Item().Text($"Transferencias: {totales.TotalTransferencias} por {totales.MontoTransferencias:C}");
                        col.Item().Text($"Pagos: {totales.TotalPagos} por {totales.MontoPagos:C}");
                        col.Item().Text($"Total comisiones: {totales.TotalComisiones:C}");
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().Text("Top 10 clientes por volumen").SemiBold().FontSize(12);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn();
                                cols.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Cliente").SemiBold();
                                header.Cell().Text("Volumen").SemiBold();
                            });

                            foreach (var c in top10)
                            {
                                table.Cell().Text(c.NombreCompleto);
                                table.Cell().Text(c.VolumenTotal.ToString("C"));
                            }
                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().Text("Volumen diario").SemiBold().FontSize(12);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn();
                                cols.RelativeColumn();
                                cols.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Fecha").SemiBold();
                                header.Cell().Text("Cantidad").SemiBold();
                                header.Cell().Text("Monto").SemiBold();
                            });

                            foreach (var d in volumenDiario)
                            {
                                table.Cell().Text(d.Fecha.ToString("dd/MM/yyyy"));
                                table.Cell().Text(d.CantidadTransacciones.ToString());
                                table.Cell().Text(d.MontoTotal.ToString("C"));
                            }
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        // ========== RF-G1: Excel (CSV) ==========
        public async Task<byte[]> GenerarReporteExcelAsync(DateTime desde, DateTime hasta)
        {
            var totales = await ObtenerTotalesAsync(desde, hasta);
            var top10 = await ObtenerTop10ClientesAsync(desde, hasta);
            var volumenDiario = await ObtenerVolumenDiarioAsync(desde, hasta);

            var sb = new StringBuilder();

            sb.AppendLine($"Reporte de operaciones;{desde:dd/MM/yyyy};{hasta:dd/MM/yyyy}");
            sb.AppendLine();
            sb.AppendLine("Totales");
            sb.AppendLine($"Total transferencias;{totales.TotalTransferencias};{totales.MontoTransferencias}");
            sb.AppendLine($"Total pagos;{totales.TotalPagos};{totales.MontoPagos}");
            sb.AppendLine($"Total comisiones;;{totales.TotalComisiones}");

            sb.AppendLine();
            sb.AppendLine("Top 10 clientes por volumen");
            sb.AppendLine("Cliente;Volumen");
            foreach (var c in top10)
            {
                sb.AppendLine($"{c.NombreCompleto};{c.VolumenTotal}");
            }

            sb.AppendLine();
            sb.AppendLine("Volumen diario");
            sb.AppendLine("Fecha;Cantidad;Monto");
            foreach (var d in volumenDiario)
            {
                sb.AppendLine($"{d.Fecha:dd/MM/yyyy};{d.CantidadTransacciones};{d.MontoTotal}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
