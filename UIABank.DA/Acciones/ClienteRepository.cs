using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.DA;
using UIABank.DA.Config;

namespace UIABank.DA.Acciones
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly UIABankDbContext _context;

        public ClienteRepository(UIABankDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> ObtenerPorIdsAsync(List<int> idsClientes)
        {
            return await _context.Clientes
                .Where(c => idsClientes.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<Cliente> ObtenerPorIdentificacionAsync(string identificacion)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Identificacion == identificacion);
        }

        public async Task<bool> ExisteIdentificacionAsync(string identificacion)
        {
            return await _context.Clientes.AnyAsync(c => c.Identificacion == identificacion);
        }

        public async Task<Cliente> CrearAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        // ✅ MÉTODO CORREGIDO 1
        public async Task ActualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            return await _context.Clientes
                .Include(c => c.Usuario)
                .ToListAsync();
        }

        // ✅ MÉTODO IMPLEMENTADO 1
        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return await _context.Clientes
                .Include(c => c.Usuario) // Incluye el usuario asociado si existe
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // ✅ MÉTODO IMPLEMENTADO 2
        Task<Cliente?> IClienteRepository.ActualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            _context.SaveChanges(); // Síncrono porque la interfaz lo requiere así
            return Task.FromResult<Cliente?>(cliente);
        }
    }
}
