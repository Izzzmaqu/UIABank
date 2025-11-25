using Microsoft.EntityFrameworkCore;
using UIABank.BC.Beneficiarios;
using UIABank.BC.Cuentas;
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
        }
    }
}

