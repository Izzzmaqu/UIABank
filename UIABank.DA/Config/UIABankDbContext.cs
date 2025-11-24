using Microsoft.EntityFrameworkCore;
using UIABank.BC.Modelos;

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
        public DbSet<ProveedorServicio> ProveedoresServicios { get; set; }
        public DbSet<PagoServicio> PagosServicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProveedorServicio>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Codigo).HasMaxLength(50);

                entity.Property(e => e.MinLongitudContrato).IsRequired();

                entity.Property(e => e.MaxLongitudContrato).IsRequired();

                entity.HasIndex(e => e.Nombre).IsUnique();

                entity.Property(e => e.FechaCreacion).IsRequired();
            });

            modelBuilder.Entity<PagoServicio>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.NumeroReferencia).IsRequired().HasMaxLength(50);

                entity.HasIndex(e => e.NumeroReferencia).IsUnique();

                entity.Property(e => e.NumeroContrato).IsRequired().HasMaxLength(50);

                entity.Property(e => e.Moneda).IsRequired().HasMaxLength(3);

                entity.Property(e => e.CuentaOrigen).IsRequired().HasMaxLength(30);

                entity.Property(e => e.Monto).HasColumnType("decimal(18,2)").IsRequired();

                entity.Property(e => e.FechaCreacion).IsRequired();

                entity.HasOne(e => e.ProveedorServicio).WithMany().HasForeignKey(e => e.ProveedorServicioId);

                entity.HasOne(e => e.Cliente).WithMany().HasForeignKey(e => e.ClienteId);
            });

        }
    }
}

