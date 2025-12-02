
/*using UIABank.BC.Beneficiarios;
using UIABank.BC.Modelos;
using UIABank.DA.Config;


namespace UIABank.DA.Repositorios
{
    public class BeneficiarioRepository : IBeneficiarioRepository
    {
        private readonly UIABankDbContext _context;

        public BeneficiarioRepository(UIABankDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AliasExisteParaClienteAsync(Guid clienteId, string alias)
        {
            var aliasNormalizado = alias.Trim();
            return await _context.Beneficiarios
                .AnyAsync(b => b.ClienteId == clienteId && b.Alias == aliasNormalizado);
        }

        public async Task AgregarAsync(Beneficiario beneficiario)
        {
            await _context.Beneficiarios.AddAsync(beneficiario);
        }

        public async Task<Beneficiario?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Beneficiarios.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Beneficiario>> ObtenerPorClienteAsync(
            Guid clienteId,
            string? alias,
            string? banco,
            string? pais)
        {
            var query = _context.Beneficiarios
                .Where(b => b.ClienteId == clienteId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(alias))
            {
                var a = alias.Trim();
                query = query.Where(b => b.Alias.Contains(a));
            }

            if (!string.IsNullOrWhiteSpace(banco))
            {
                var bco = banco.Trim();
                query = query.Where(b => b.Banco.Contains(bco));
            }

            if (!string.IsNullOrWhiteSpace(pais))
            {
                var p = pais.Trim();
                query = query.Where(b => b.Pais.Contains(p));
            }

            return await query.ToListAsync();
        }

        public async Task EliminarAsync(Beneficiario beneficiario)
        {
            _context.Beneficiarios.Remove(beneficiario);
            await Task.CompletedTask;
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
*/