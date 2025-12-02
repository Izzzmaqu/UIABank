using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;

namespace UIABank.BW.CU
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditoriaService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public Task RegistrarEventoAsync(
            string usuario,
            string categoria,
            string accion,
            string detalles,
            string? ip)
        {
            var log = new AuditLog
            {
                Usuario = usuario,
                Categoria = categoria,
                Accion = accion,
                Detalles = detalles,
                Ip = ip,
                Fecha = DateTime.UtcNow
            };

            return _auditLogRepository.RegistrarAsync(log);
        }

        public Task<List<AuditLog>> ObtenerEventosAsync(DateTime desde, DateTime hasta)
        {
            return _auditLogRepository.ObtenerPorRangoAsync(desde, hasta);
        }
    }
}
