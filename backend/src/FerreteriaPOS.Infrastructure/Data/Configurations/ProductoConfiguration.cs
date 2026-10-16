using FerreteriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerreteriaPOS.Infrastructure.Data.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("productos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.SKU)
            .IsUnique();

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descripcion)
            .HasMaxLength(1000);

        builder.Property(p => p.CodigoBarras)
            .HasMaxLength(50);

        builder.HasIndex(p => p.CodigoBarras);

        builder.Property(p => p.CategoriaId)
            .IsRequired();

        builder.Property(p => p.Variante)
            .HasMaxLength(100);

        builder.Property(p => p.StockActual)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.StockMinimo)
            .IsRequired()
            .HasDefaultValue(5);

        builder.Property(p => p.UnidadMedida)
            .HasMaxLength(50);

        builder.Property(p => p.PermitirRecalculo)
            .IsRequired()
            .HasDefaultValue(true);

        // Relaciones
        builder.HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Precios)
            .WithOne(pp => pp.Producto)
            .HasForeignKey(pp => pp.ProductoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Movimientos)
            .WithOne(m => m.Producto)
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.FormulaPersonalizada)
            .WithOne(f => f.Producto)
            .HasForeignKey<FormulaPrecios>(f => f.ProductoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(p => p.CreadoEn)
            .IsRequired();

        builder.Property(p => p.ModificadoEn);

        builder.Property(p => p.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
