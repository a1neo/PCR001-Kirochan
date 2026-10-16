using FerreteriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerreteriaPOS.Infrastructure.Data.Configurations;

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("ventas");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.NumeroVenta)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(v => v.NumeroVenta)
            .IsUnique();

        builder.Property(v => v.FechaVenta)
            .IsRequired();

        builder.HasIndex(v => v.FechaVenta);

        builder.Property(v => v.VendedorId)
            .IsRequired();

        builder.Property(v => v.Subtotal)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(v => v.Total)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(v => v.MonedaPago)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(v => v.MontoPagado)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(v => v.Cambio)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(v => v.TipoCambioAplicado)
            .HasColumnType("decimal(18,4)");

        // Relaciones
        builder.HasOne(v => v.Vendedor)
            .WithMany()
            .HasForeignKey(v => v.VendedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(v => v.Detalles)
            .WithOne(d => d.Venta)
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(v => v.CreadoEn)
            .IsRequired();

        builder.Property(v => v.ModificadoEn);

        builder.Property(v => v.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}

public class DetalleVentaConfiguration : IEntityTypeConfiguration<DetalleVenta>
{
    public void Configure(EntityTypeBuilder<DetalleVenta> builder)
    {
        builder.ToTable("detalles_ventas");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.VentaId)
            .IsRequired();

        builder.Property(d => d.ProductoId)
            .IsRequired();

        builder.Property(d => d.ProductoNombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.ProductoSKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Cantidad)
            .IsRequired();

        builder.Property(d => d.PrecioUnitario)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(d => d.MonedaPrecio)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.Subtotal)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Relación con Producto (para reportes)
        builder.HasOne(d => d.Producto)
            .WithMany()
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(d => d.VentaId);
        builder.HasIndex(d => d.ProductoId);

        builder.Property(d => d.CreadoEn)
            .IsRequired();

        builder.Property(d => d.ModificadoEn);

        builder.Property(d => d.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
