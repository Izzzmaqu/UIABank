using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UIABank.BC.Modelos;

namespace UIABank.DA.Configurations
{
    public class BeneficiarioConfiguration : IEntityTypeConfiguration<Beneficiario>
    {
        public void Configure(EntityTypeBuilder<Beneficiario> builder)
        {
            builder.ToTable("Beneficiarios");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.ClienteId)
                .IsRequired();

            builder.Property(b => b.Alias)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(b => b.Banco)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.NumeroCuenta)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(b => b.Pais)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Moneda)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(b => b.Estado)
                .IsRequired()
                .HasConversion<int>();

            // Alias único por cliente
            builder.HasIndex(b => new { b.ClienteId, b.Alias })
                .IsUnique();
        }
    }
}