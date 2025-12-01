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

        public Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<Cliente?> IClienteRepository.ActualizarAsync(Cliente cliente)
        {
            throw new NotImplementedException();
        }
    }
}

