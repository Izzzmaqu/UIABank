using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;

namespace UIABank.BW.CU
{
    public class TransferenciaProgramadaBW : ITransferenciaProgramadaBW
    {
        private readonly ITransferenciaProgramadaDA transferenciaProgramadaDA;
        private readonly ITransferenciaBW transferenciaBW;

        public TransferenciaProgramadaBW(
            ITransferenciaProgramadaDA transferenciaProgramadaDA,
            ITransferenciaBW transferenciaBW)
        {
            this.transferenciaProgramadaDA = transferenciaProgramadaDA;
            this.transferenciaBW = transferenciaBW;
        }

        // RF-D3: Crear transferencia programada (max 90 días, futuro)
        public async Task<bool> CrearProgramada(Transferencia transferencia)
        {
            if (!transferencia.FechaProgramada.HasValue)
                return false;

            var ahora = DateTime.Now;
            var fecha = transferencia.FechaProgramada.Value;

            if (fecha <= ahora)
                return false;

            if (fecha > ahora.AddDays(90))
                return false;

            transferencia.Estado = EstadoTransferencia.Programada;
            transferencia.FechaCreacion = ahora;

            return await transferenciaProgramadaDA.CrearAsync(transferencia);
        }

        // Obtener solo las Transferencias (para que el front pueda mostrar la lista)
        public async Task<IEnumerable<Transferencia>> ObtenerPendientesAsync()
        {
           
            var pendientes = await transferenciaProgramadaDA.ObtenerPendientesAsync();
            return pendientes.Select(p => p.Transferencia);
        }

        // Job scheduler: ejecutar todas las programadas vencidas
        public async Task EjecutarPendientesAsync()
        {
            var pendientes = await transferenciaProgramadaDA.ObtenerPendientesAsync();

            foreach (var p in pendientes)
            {
                // p.Transferencia es la transferencia real a ejecutar
                var ok = await transferenciaBW.EjecutarAsync(p.Transferencia);

                // Si se ejecutó, marcamos estado y actualizamos en DA
                p.Transferencia.Estado = ok
                    ? EstadoTransferencia.Exitosa
                    : EstadoTransferencia.Fallida;

                await transferenciaProgramadaDA.ActualizarAsync(p);
            }
        }

        // RF-D3: Cancelar programada hasta 24h antes
        public async Task<bool> CancelarProgramadaAsync(int id)
        {
            var programacion = await transferenciaProgramadaDA.ObtenerPorIdAsync(id);
            if (programacion == null)
                return false;

            if (programacion.Ejecutada || programacion.Cancelada)
                return false;

            if (programacion.FechaProgramada <= DateTime.Now.AddHours(24))
                return false;

            programacion.Cancelada = true;

            if (programacion.Transferencia != null)
                programacion.Transferencia.Estado = EstadoTransferencia.Cancelada;

            return await transferenciaProgramadaDA.ActualizarAsync(programacion);
        }
    }
}