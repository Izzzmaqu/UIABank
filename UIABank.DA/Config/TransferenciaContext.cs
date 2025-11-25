using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;
using UIABank.BC.Cuentas;

namespace UIABank.DA.Config
{

    public class TransferenciaContext : DbContext
    {
        public TransferenciaContext(DbContextOptions<TransferenciaContext> options)
            : base(options) { }

    
        public DbSet<Transferencia> Transferencias { get; set; }
        public DbSet<Cuenta> Cuenta { get; set; }

        public DbSet<ProgramacionTransferencia> Programaciones { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
        }
    }
}
