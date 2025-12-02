using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.DA
{
    public interface IAuditLogRepository
    {
        Task RegistrarAsync(AuditLog log);
        Task<List<AuditLog>> ObtenerPorRangoAsync(DateTime desde, DateTime hasta);
    }
}

