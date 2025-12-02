using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.DA;
using UIABank.DA.Config;

namespace UIABank.DA.Acciones
{
    public class AuditLogRepository : IAuditLogRepository
    {
        // TODO: cambia BankContext por el DbContext real (por ejemplo TransferenciaContext)
        private readonly UIABankDbContext _context;

        public AuditLogRepository(UIABankDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public Task<List<AuditLog>> ObtenerPorRangoAsync(DateTime desde, DateTime hasta)
        {
            return _context.AuditLogs
                .Where(a => a.Fecha >= desde && a.Fecha <= hasta)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();
        }
    }
}
