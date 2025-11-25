using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;

namespace UIABank.BW.CU
{
    public class PagoServicioService : IPagoServicioService
    {
        private readonly IPagoServicioRepository _pagoRepo;
        private readonly IProveedorServicioRepository _provRepo;
        private readonly IClienteRepository _clienteRepo;

        public PagoServicioService(
            IPagoServicioRepository pagoRepo,
            IProveedorServicioRepository provRepo,
            IClienteRepository clienteRepo)
        {
            _pagoRepo = pagoRepo;
            _provRepo = provRepo;
            _clienteRepo = clienteRepo;
        }

        public async Task<PagoServicioResultadoDto> CrearPagoAsync(CrearPagoServicioDto dto)
        {
            // 1. Validar cliente
            var cliente = await _clienteRepo.ObtenerPorIdAsync(dto.ClienteId)
                ?? throw new ArgumentException("Cliente no encontrado");

            // 2. Validar proveedor
            var proveedor = await _provRepo.ObtenerPorIdAsync(dto.ProveedorServicioId)
                ?? throw new ArgumentException("Proveedor no encontrado");

            if (!proveedor.Activo)
                throw new InvalidOperationException("El proveedor está inactivo");

            // 3. Validar contrato según reglas del proveedor
            if (string.IsNullOrWhiteSpace(dto.NumeroContrato))
                throw new ArgumentException("El número de contrato es obligatorio");

            var contrato = dto.NumeroContrato.Trim();

            if (!Regex.IsMatch(contrato, @"^\d+$"))
                throw new ArgumentException("El número de contrato debe contener solo dígitos");

            if (contrato.Length < proveedor.MinLongitudContrato ||
                contrato.Length > proveedor.MaxLongitudContrato)
                throw new ArgumentException(
                    $"El número de contrato debe tener entre {proveedor.MinLongitudContrato} y {proveedor.MaxLongitudContrato} dígitos");

            // 4. Validar monto y moneda
            if (dto.Monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a 0");

            if (string.IsNullOrWhiteSpace(dto.Moneda) ||
                (dto.Moneda != "CRC" && dto.Moneda != "USD"))
                throw new ArgumentException("Moneda inválida. Debe ser CRC o USD");

            if (string.IsNullOrWhiteSpace(dto.CuentaOrigen))
                throw new ArgumentException("La cuenta origen es obligatoria");

            // TODO: aquí se debe validar saldo y límite diario
            //   - consultar módulo de cuentas
            //   - validar saldo suficiente
            //   - validar que no se exceda el límite diario

            // 5. Definir estado inicial
            var ahora = DateTime.UtcNow;
            EstadoPagoServicio estadoInicial;
            DateTime? fechaProg = null;

            if (dto.FechaProgramada.HasValue)
            {
                // Pago programado
                if (dto.FechaProgramada.Value <= ahora)
                    throw new ArgumentException("La fecha programada debe ser futura");

                estadoInicial = EstadoPagoServicio.Programado;
                fechaProg = dto.FechaProgramada.Value;
            }
            else
            {
                // Pago inmediato (para simplificar: se marca como Exitoso directamente)
                estadoInicial = EstadoPagoServicio.Exitoso;
            }

            // 6. Generar número de referencia único
            var numeroReferencia = GenerarNumeroReferencia();

            var pago = new PagoServicio
            {
                ClienteId = cliente.Id,
                ProveedorServicioId = proveedor.Id,
                NumeroContrato = contrato,
                Monto = dto.Monto,
                Moneda = dto.Moneda,
                CuentaOrigen = dto.CuentaOrigen.Trim(),
                FechaCreacion = ahora,
                FechaProgramada = fechaProg,
                Estado = estadoInicial,
                NumeroReferencia = numeroReferencia
            };

            await _pagoRepo.CrearAsync(pago);

            return new PagoServicioResultadoDto
            {
                PagoServicioId = pago.Id,
                NumeroReferencia = pago.NumeroReferencia,
                Estado = pago.Estado
            };
        }

        public Task<List<PagoServicio>> ObtenerPagosClienteAsync(
            int clienteId,
            DateTime? desde,
            DateTime? hasta,
            bool soloProgramados)
        {
            return _pagoRepo.ObtenerPorClienteAsync(clienteId, desde, hasta, soloProgramados);
        }

        public async Task CancelarPagoProgramadoAsync(int pagoServicioId, int clienteId)
        {
            var pago = await _pagoRepo.ObtenerPorIdAsync(pagoServicioId)
                ?? throw new ArgumentException("Pago no encontrado");

            if (pago.ClienteId != clienteId)
                throw new InvalidOperationException("No puede cancelar pagos de otro cliente");

            if (pago.Estado != EstadoPagoServicio.Programado)
                throw new InvalidOperationException("Solo se pueden cancelar pagos programados");

            var ahora = DateTime.UtcNow;

            if (pago.FechaProgramada.HasValue &&
                pago.FechaProgramada.Value <= ahora.AddHours(24))
            {
                throw new InvalidOperationException(
                    "Los pagos solo se pueden cancelar hasta 24 horas antes de la fecha programada");
            }

            pago.Estado = EstadoPagoServicio.Cancelado;
            await _pagoRepo.ActualizarAsync(pago);
        }

        private string GenerarNumeroReferencia()
        {
            // PS-YYYYMMDD-HHMMSS-XXXX
            var ahora = DateTime.UtcNow;
            var random = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant();
            return $"PS-{ahora:yyyyMMddHHmmss}-{random}";
        }
    }
}
