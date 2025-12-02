using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.BW
{
    public interface IAuditoriaService
    {
        Task RegistrarEventoAsync(
            string usuario,
            string categoria,
            string accion,
            string detalles,
            string? ip);

        Task<List<AuditLog>> ObtenerEventosAsync(DateTime desde, DateTime hasta);
    }
}
