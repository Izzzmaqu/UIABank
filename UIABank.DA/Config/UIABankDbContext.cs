using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;
using UIABank.DA.Configurations;

namespace UIABank.DA.Config
{
    public class UIABankDbContext : DbContext
    {
        public UIABankDbContext(DbContextOptions<UIABankDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Beneficiario> Beneficiarios { get; set; }
        public DbSet<PagoServicio> PagosServicios { get; set; }
        public DbSet<Transferencia> Transferencias { get; set; }
        public DbSet<ProgramacionTransferencia> ProgramacionesTransferencias { get; set; }
        public DbSet<ProveedorServicio> ProveedoresServicios { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Rol).IsRequired().HasMaxLength(50);

                entity.HasOne(e => e.Cliente)
                    .WithOne(c => c.Usuario)
                    .HasForeignKey<Usuario>(e => e.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Identificacion).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Identificacion).IsUnique();
                entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Correo).IsRequired().HasMaxLength(200);
            });

            modelBuilder.ApplyConfiguration(new CuentaConfiguration());
            modelBuilder.ApplyConfiguration(new BeneficiarioConfiguration());

            modelBuilder.Entity<PagoServicio>()
                .Property(p => p.Monto)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transferencia>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Monto).HasPrecision(18, 2);
                entity.Property(t => t.Comision).HasPrecision(18, 2);

                entity.HasOne(t => t.CuentaOrigen)
                    .WithMany()
                    .HasForeignKey(t => t.CuentaOrigenId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.CuentaDestino)
                    .WithMany()
                    .HasForeignKey(t => t.CuentaDestinoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Tercero)
                    .WithMany()
                    .HasForeignKey(t => t.TerceroId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => t.IdempotencyKey).IsUnique();
                entity.HasIndex(t => t.Referencia).IsUnique();
            });

            modelBuilder.Entity<ProgramacionTransferencia>(entity =>
            {
                entity.HasKey(p => p.Id);
            });
        }
    }
}
