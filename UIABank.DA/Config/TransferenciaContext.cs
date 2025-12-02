using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;

namespace UIABank.DA.Config
{

    public class TransferenciaContext : DbContext
    {
        public TransferenciaContext(DbContextOptions<TransferenciaContext> options)
            : base(options) { }

    
        public DbSet<Transferencia> Transferencias { get; set; }
        public DbSet<Cuenta> Cuenta { get; set; }

        public DbSet<ProgramacionTransferencia> Programaciones { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
        }
    }
}
