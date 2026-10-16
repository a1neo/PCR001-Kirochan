using FerreteriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerreteriaPOS.Infrastructure.Data.Configurations;

public class PrecioProductoConfiguration : IEntityTypeConfiguration<PrecioProducto>
{
    public void Configure(EntityTypeBuilder<PrecioProducto> builder)
    {
        builder.ToTable("precios_productos");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.ProductoId)
            .IsRequired();

        builder.Property(pp => pp.TipoPrecio)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pp => pp.Valor)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(pp => pp.Moneda)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pp => pp.FechaVigencia)
            .IsRequired();

        // Índice compuesto: un producto solo puede tener un precio activo por tipo
        builder.HasIndex(pp => new { pp.ProductoId, pp.TipoPrecio, pp.Activo });

        builder.Property(pp => pp.CreadoEn)
            .IsRequired();

        builder.Property(pp => pp.ModificadoEn);

        builder.Property(pp => pp.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
