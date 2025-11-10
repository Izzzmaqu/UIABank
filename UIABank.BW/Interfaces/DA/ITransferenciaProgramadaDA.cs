using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.DA
{
    public interface ITransferenciaProgramadaDA
    {
        // Crear una programación a partir de la entidad de programación
        Task<bool> CrearAsync(ProgramacionTransferencia programacion);

        // Crear una programación a partir de la transferencia 
        Task<bool> CrearAsync(Transferencia transferencia);

        // Obtener todas las programaciones pendientes de ejecución
        Task<IEnumerable<ProgramacionTransferencia>> ObtenerPendientesAsync();

        // Obtener una programación por Id 
        Task<ProgramacionTransferencia?> ObtenerPorIdAsync(int id);

        // Actualizar una programación 
        Task<bool> ActualizarAsync(ProgramacionTransferencia programacion);

    
        Task<bool> MarcarEjecutada(int id);
        Task<bool> Cancelar(int id);
    }
}

