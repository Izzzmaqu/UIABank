using Microsoft.EntityFrameworkCore;
using UIABank.BC.Cuentas;
using UIABank.DA.Config;

namespace UIABank.DA.Repositorios
{
    public class CuentaRepository : ICuentaRepository
    {
        private readonly UIABankDbContext _context;

        public CuentaRepository(UIABankDbContext context)
        {
            _context = context;
        }

        public async Task<int> ContarCuentasPorClienteTipoMonedaAsync(Guid clienteId, TipoCuenta tipo, Moneda moneda)
        {
            return await _context.Cuentas
                .Where(c => c.ClienteId == clienteId
                            && c.Tipo == tipo
                            && c.Moneda == moneda)
                .CountAsync();
        }

        public async Task<bool> ExisteNumeroCuentaAsync(string numero)
        {
            return await _context.Cuentas.AnyAsync(c => c.Numero == numero);
        }

        public async Task AgregarAsync(Cuenta cuenta)
        {
            await _context.Cuentas.AddAsync(cuenta);
        }

        public async Task<Cuenta?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Cuentas.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Cuenta>> ObtenerPorClienteAsync(Guid clienteId)
        {
            return await _context.Cuentas
                .Where(c => c.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<List<Cuenta>> BuscarAsync(
            Guid? clienteId,
            TipoCuenta? tipo,
            Moneda? moneda,
            EstadoCuenta? estado)
        {
            var query = _context.Cuentas.AsQueryable();

            if (clienteId.HasValue)
                query = query.Where(c => c.ClienteId == clienteId.Value);

            if (tipo.HasValue)
                query = query.Where(c => c.Tipo == tipo.Value);

            if (moneda.HasValue)
                query = query.Where(c => c.Moneda == moneda.Value);

            if (estado.HasValue)
                query = query.Where(c => c.Estado == estado.Value);

            return await query.ToListAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
