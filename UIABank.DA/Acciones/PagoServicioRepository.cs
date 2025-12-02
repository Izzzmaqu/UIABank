using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.DA;
using UIABank.DA.Config;

namespace UIABank.DA.Acciones
{
    public class PagoServicioRepository : IPagoServicioRepository
    {
        private readonly UIABankDbContext _context;

        public PagoServicioRepository(UIABankDbContext context)
        {
            _context = context;
        }

        public async Task<PagoServicio> CrearAsync(PagoServicio pago)
        {
            _context.PagosServicios.Add(pago);
            await _context.SaveChangesAsync();
            return pago;
        }

        public async Task<PagoServicio> ObtenerPorIdAsync(int id)
        {
            return await _context.PagosServicios
                .Include(p => p.ProveedorServicio)
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PagoServicio>> ObtenerPorClienteAsync(
            int clienteId,
            DateTime? desde,
            DateTime? hasta,
            bool soloProgramados)
        {
            var query = _context.PagosServicios
                .Include(p => p.ProveedorServicio)
                .Where(p => p.ClienteId == clienteId)
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(p => p.FechaCreacion >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(p => p.FechaCreacion <= hasta.Value);

            if (soloProgramados)
                query = query.Where(p => p.Estado == EstadoPagoServicio.Programado);

            return await query
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        // ← NUEVO MÉTODO AQUÍ
        public async Task<List<PagoServicio>> ObtenerTodosAsync(
            DateTime? desde,
            DateTime? hasta,
            bool soloProgramados)
        {
            var query = _context.PagosServicios
                .Include(p => p.Cliente)
                .Include(p => p.ProveedorServicio)
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(p => p.FechaCreacion >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(p => p.FechaCreacion <= hasta.Value);

            if (soloProgramados)
                query = query.Where(p => p.Estado == EstadoPagoServicio.Programado);

            return await query
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task ActualizarAsync(PagoServicio pago)
        {
            _context.PagosServicios.Update(pago);
            await _context.SaveChangesAsync();
        }

        public Task<List<PagoServicio>> ListarPorRangoFechasAsync(DateTime desde, DateTime hasta)
        {
            return _context.PagosServicios
                .Where(p => p.FechaEjecucion >= desde &&
                            p.FechaEjecucion <= hasta)
                .ToListAsync();
        }
    }
}
