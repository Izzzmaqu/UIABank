using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UIABank.BC.Cuentas;

namespace UIABank.DA.Configurations
{
    public class CuentaConfiguration : IEntityTypeConfiguration<Cuenta>
    {
        public void Configure(EntityTypeBuilder<Cuenta> builder)
        {
            builder.ToTable("Cuentas");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Numero)
                .IsRequired()
                .HasMaxLength(12)
                .IsUnicode(false);

            // Índice único para el número de cuenta
            builder.HasIndex(c => c.Numero)
                .IsUnique();

            builder.Property(c => c.Saldo)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.Tipo)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.Moneda)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.ClienteId)
                .IsRequired();

            // Cuando tengas la entidad Cliente, aquí puedes definir la relación:
            // builder.HasOne<Cliente>()
            //     .WithMany() // o .WithMany(x => x.Cuentas)
            //     .HasForeignKey(c => c.ClienteId);
        }
    }
}