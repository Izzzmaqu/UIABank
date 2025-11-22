using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.BW
{
    public interface ITransferenciaProgramadaBW
    {
        // Crear una transferencia programada (RF-D3)
        Task<bool> CrearProgramada(Transferencia transferencia);

        // Consultar las transferencias programadas pendientes de ejecución
        Task<IEnumerable<Transferencia>> ObtenerPendientesAsync();

        // Ejecutar todas las transferencias programadas que ya cumplieron la fecha/hora
        Task EjecutarPendientesAsync();

        // Cancelar una transferencia programada hasta 24h antes de la ejecución
        Task<bool> CancelarProgramadaAsync(int id);
    }
}

