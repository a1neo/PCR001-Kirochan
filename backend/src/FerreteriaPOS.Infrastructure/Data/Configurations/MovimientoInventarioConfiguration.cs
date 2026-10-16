using FerreteriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerreteriaPOS.Infrastructure.Data.Configurations;

public class MovimientoInventarioConfiguration : IEntityTypeConfiguration<MovimientoInventario>
{
    public void Configure(EntityTypeBuilder<MovimientoInventario> builder)
    {
        builder.ToTable("movimientos_inventario");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ProductoId)
            .IsRequired();

        builder.Property(m => m.TipoMovimiento)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.Cantidad)
            .IsRequired();

        builder.Property(m => m.PrecioUnitario)
            .HasColumnType("decimal(18,2)");

        builder.Property(m => m.MonedaPrecio)
            .HasConversion<int>();

        builder.Property(m => m.Motivo)
            .HasMaxLength(500);

        builder.Property(m => m.RegistradoPorId)
            .IsRequired();

        builder.Property(m => m.VentaId);

        // Relaciones
        builder.HasOne(m => m.RegistradoPor)
            .WithMany()
            .HasForeignKey(m => m.RegistradoPorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Venta)
            .WithMany()
            .HasForeignKey(m => m.VentaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(m => m.ProductoId);
        builder.HasIndex(m => m.VentaId);
        builder.HasIndex(m => m.CreadoEn);

        builder.Property(m => m.CreadoEn)
            .IsRequired();

        builder.Property(m => m.ModificadoEn);

        builder.Property(m => m.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
