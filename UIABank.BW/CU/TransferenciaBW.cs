using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Modelos;
using UIABank.BC.ReglasDeNegocio;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;



namespace UIABank.BW.CU
{
    public class TransferenciaBW : ITransferenciaBW
    {
        private readonly ITransferenciaDA transferenciaDA;
        private readonly ICuentaDA cuentaDA;


        public TransferenciaBW(ITransferenciaDA transferenciaDA, ICuentaDA cuentaDA)
        {
            this.transferenciaDA = transferenciaDA;
            this.cuentaDA = cuentaDA;
        }


        // RF-D1: PRECHECK
        public async Task<bool> CrearAsync(Transferencia transferencia)
        {
            // 1. Validación de monto (> 0)
            if (!ReglasTransferencia.ValidarMontoPositivo(transferencia.Monto))
                return false;

            // 2. Obtener cuenta origen 
            var cuentaOrigen = await cuentaDA.ObtenerPorIdAsync(transferencia.CuentaOrigenId);
            if (cuentaOrigen == null)
                return false;

            // 3. Estado activo
            if (!ReglasTransferencia.ValidarEstadoCuenta(cuentaOrigen.Estado))
                return false;

            // 4. Saldo suficiente (saldo >= monto + comisión)
            if (!ReglasTransferencia.ValidarSaldo(
                    cuentaOrigen.Saldo,
                    transferencia.Monto,
                    transferencia.Comision))
                return false;

            // 5. Moneda igual
            if (!ReglasTransferencia.ValidarMoneda(
                    cuentaOrigen.Moneda,
                    transferencia.Moneda))
                return false;

            // 6. Límite diario 
            const decimal LIMITE_DIARIO = 100000m;

            var totalHoy = await transferenciaDA.ObtenerTotalDiarioAsync(
                transferencia.UsuarioEjecutorId,
                DateTime.Today,
                transferencia.Moneda
            );

            if (totalHoy + transferencia.Monto > LIMITE_DIARIO)
                return false; // supera el límite diario permitido

            // 7. Tercero confirmado 
            if (transferencia.TerceroId.HasValue)
            {
                
            }

            // 8. Si pasa todas las validaciones, se guarda la transferencia en BD
            return await transferenciaDA.CrearAsync(transferencia);
        }

        // RF-D2: EJECUTAR TRANSFERENCIA REAL

        public async Task<bool> EjecutarAsync(Transferencia transferencia)
        {
            // 1. Obtener cuenta origen
            var cuentaOrigen = await cuentaDA.ObtenerPorIdAsync(transferencia.CuentaOrigenId);
            if (cuentaOrigen == null || cuentaOrigen.Estado != "Activa")
                return false;

            // 2. Obtener cuenta destino si existe
            Cuenta? cuentaDestino = null;
            if (transferencia.CuentaDestinoId != null)
            {
                cuentaDestino = await cuentaDA.ObtenerPorIdAsync(transferencia.CuentaDestinoId.Value);
            }

            // 3. Validaciones
            if (!ReglasTransferencia.ValidarSaldo(cuentaOrigen.Saldo, transferencia.Monto, transferencia.Comision))
                return false;

            if (!ReglasTransferencia.ValidarMoneda(cuentaOrigen.Moneda, transferencia.Moneda))
                return false;

            // 4. Idempotencia
            var trxExistente = await transferenciaDA.ObtenerPorIdempotencyKeyAsync(transferencia.IdempotencyKey);
            if (trxExistente != null)
                return true; 

            // 5. Estado según monto
            transferencia.Estado =
                transferencia.Monto > 50000
                    ? EstadoTransferencia.PendienteAprobacion
                    : EstadoTransferencia.Exitosa;

            // 6. Delegar ejecución real a la DA
            return await transferenciaDA.EjecutarTransferenciaAsync(
                transferencia,
                cuentaOrigen,
                cuentaDestino
            );
        }

        public Task<bool> ActualizarEstadoAsync(int id, EstadoTransferencia nuevoEstado)
            => transferenciaDA.ActualizarEstadoAsync(id, nuevoEstado);

        public Task<Transferencia?> ObtenerPorIdAsync(int id)
            => transferenciaDA.ObtenerPorIdAsync(id);

        public Task<IEnumerable<Transferencia>> ListarPorUsuarioAsync(int usuarioId)
            => transferenciaDA.ListarPorUsuarioAsync(usuarioId);
    }
}
