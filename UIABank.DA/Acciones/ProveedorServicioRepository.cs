using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.DA;
using UIABank.DA.Config;

namespace UIABank.DA.Acciones
{
    public class ProveedorServicioRepository : IProveedorServicioRepository
    {
        private readonly UIABankDbContext _context;

        public ProveedorServicioRepository(UIABankDbContext context)
        {
            _context = context;
        }

        public async Task<ProveedorServicio> CrearAsync(ProveedorServicio proveedor)
        {
            _context.ProveedoresServicios.Add(proveedor);
            await _context.SaveChangesAsync();
            return proveedor;
        }

        public async Task<ProveedorServicio> ObtenerPorIdAsync(int id)
        {
            return await _context.ProveedoresServicios
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ProveedorServicio>> ObtenerTodosAsync()
        {
            return await _context.ProveedoresServicios
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task ActualizarAsync(ProveedorServicio proveedor)
        {
            _context.ProveedoresServicios.Update(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteNombreAsync(string nombre)
        {
            return await _context.ProveedoresServicios
                .AnyAsync(p => p.Nombre == nombre);
        }
    }
}
