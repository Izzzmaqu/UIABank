using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.DA;
using UIABank.DA.Config;

namespace UIABank.DA.Acciones
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UIABankDbContext _context;

        public UsuarioRepository(UIABankDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<Usuario> CrearAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}

