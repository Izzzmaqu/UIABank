using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Cuentas;
using UIABank.BC.Modelos;
using UIABank.BC.ReglasDeNegocio;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;
using UIABank.BC.ReglasDeNegocio;
namespace UIABank.BW.CU
{
    public class TransferenciaBW : ITransferenciaBW
    {
        private readonly ITransferenciaDA _transferenciaDA;
        private readonly ICuentaRepository _cuentaRepository;

        public TransferenciaBW(ITransferenciaDA transferenciaDA,
                               ICuentaRepository cuentaRepository)
        {
            _transferenciaDA = transferenciaDA;
            _cuentaRepository = cuentaRepository;
        }

        // RF-D1: PRECHECK
        public async Task<bool> CrearAsync(Transferencia transferencia)
        {
            // 1. Validación de monto (> 0)
            if (!ReglasTransferencia.ValidarMontoPositivo(transferencia.Monto))
                return false;

            // 2. Obtener cuenta origen 
            var cuentaOrigen = await _cuentaRepository.ObtenerPorIdAsync(transferencia.CuentaOrigenId);
            if (cuentaOrigen is null)
                return false;

            // 3. Estado activo
            if (!ReglasTransferencia.ValidarEstadoCuenta(cuentaOrigen.Estado.ToString()))
                return false;

            // 4. Saldo suficiente (saldo >= monto + comisión)
            if (!ReglasTransferencia.ValidarSaldo(
                    cuentaOrigen.Saldo,
                    transferencia.Monto,
                    transferencia.Comision))
                return false;

            // 5. Moneda igual
            if (!ReglasTransferencia.ValidarMoneda(
                    cuentaOrigen.Moneda.ToString(),
                    transferencia.Moneda))
                return false;

            // 6. Límite diario 
            const decimal LIMITE_DIARIO = 100000m;

            var totalHoy = await _transferenciaDA.ObtenerTotalDiarioAsync(
                transferencia.UsuarioEjecutorId,
                DateTime.Today,
                transferencia.Moneda
            );

            if (totalHoy + transferencia.Monto > LIMITE_DIARIO)
                return false; // supera el límite diario permitido

            // 7. Tercero confirmado (cuando implementen módulo C)
            if (transferencia.TerceroId.HasValue)
            {
                // TODO: validación de beneficiario / tercero
            }

            // 8. Si pasa todas las validaciones, se guarda la transferencia en BD
            return await _transferenciaDA.CrearAsync(transferencia);
        }

        // RF-D2: EJECUTAR TRANSFERENCIA REAL
        public async Task<bool> EjecutarAsync(Transferencia transferencia)
        {
            // 1. Obtener cuenta origen
            var cuentaOrigen = await _cuentaRepository.ObtenerPorIdAsync(transferencia.CuentaOrigenId);
            if (cuentaOrigen is null)
                return false;

            if (!ReglasTransferencia.ValidarEstadoCuenta(cuentaOrigen.Estado.ToString()))
                return false;

            // 2. Obtener cuenta destino si existe
            Cuenta? cuentaDestino = null;
            if (transferencia.CuentaDestinoId.HasValue)
            {
                cuentaDestino = await _cuentaRepository.ObtenerPorIdAsync(transferencia.CuentaDestinoId.Value);
                if (cuentaDestino is null)
                    return false;
            }

            // 3. Validaciones
            if (!ReglasTransferencia.ValidarSaldo(cuentaOrigen.Saldo, transferencia.Monto, transferencia.Comision))
                return false;

            if (!ReglasTransferencia.ValidarMoneda(cuentaOrigen.Moneda.ToString(), transferencia.Moneda))
                return false;

            // 4. Idempotencia
            var trxExistente = await _transferenciaDA.ObtenerPorIdempotencyKeyAsync(transferencia.IdempotencyKey);
            if (trxExistente is not null)
                return true; // ya se procesó

            // 5. Estado según monto
            transferencia.Estado =
                transferencia.Monto > 50000
                    ? EstadoTransferencia.PendienteAprobacion
                    : EstadoTransferencia.Exitosa;

            // 6. Delegar ejecución real a la DA
            return await _transferenciaDA.EjecutarTransferenciaAsync(
                transferencia,
                cuentaOrigen,
                cuentaDestino
            );
        }

        public Task<bool> ActualizarEstadoAsync(int id, EstadoTransferencia nuevoEstado)
            => _transferenciaDA.ActualizarEstadoAsync(id, nuevoEstado);

        public Task<Transferencia?> ObtenerPorIdAsync(int id)
            => _transferenciaDA.ObtenerPorIdAsync(id);

        public Task<IEnumerable<Transferencia>> ListarPorUsuarioAsync(int usuarioId)
            => _transferenciaDA.ListarPorUsuarioAsync(usuarioId);
    }
}
