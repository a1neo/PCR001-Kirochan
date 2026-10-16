using FerreteriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerreteriaPOS.Infrastructure.Data.Configurations;

public class TipoCambioConfiguration : IEntityTypeConfiguration<TipoCambio>
{
    public void Configure(EntityTypeBuilder<TipoCambio> builder)
    {
        builder.ToTable("tipos_cambio");

        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.ValorCompra)
            .IsRequired()
            .HasColumnType("decimal(18,4)");

        builder.Property(tc => tc.ValorVenta)
            .IsRequired()
            .HasColumnType("decimal(18,4)");

        builder.Property(tc => tc.FechaVigencia)
            .IsRequired();

        builder.HasIndex(tc => tc.FechaVigencia);

        builder.Property(tc => tc.RegistradoPorId)
            .IsRequired();

        builder.HasOne(tc => tc.RegistradoPor)
            .WithMany()
            .HasForeignKey(tc => tc.RegistradoPorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(tc => tc.CreadoEn)
            .IsRequired();

        builder.Property(tc => tc.ModificadoEn);

        builder.Property(tc => tc.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
