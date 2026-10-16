using FerreteriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerreteriaPOS.Infrastructure.Data.Configurations;

public class FormulaPreciosConfiguration : IEntityTypeConfiguration<FormulaPrecios>
{
    public void Configure(EntityTypeBuilder<FormulaPrecios> builder)
    {
        builder.ToTable("formulas_precios");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.DefinicionJson)
            .IsRequired()
            .HasColumnType("jsonb"); // PostgreSQL JSONB

        builder.Property(f => f.CategoriaId);

        builder.Property(f => f.ProductoId);

        builder.Property(f => f.EsGlobal)
            .IsRequired()
            .HasDefaultValue(false);

        // Relaciones
        builder.HasOne(f => f.Categoria)
            .WithMany(c => c.FormulasPrecios)
            .HasForeignKey(f => f.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(f => f.CategoriaId);
        builder.HasIndex(f => f.ProductoId);
        builder.HasIndex(f => f.EsGlobal);

        builder.Property(f => f.CreadoEn)
            .IsRequired();

        builder.Property(f => f.ModificadoEn);

        builder.Property(f => f.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
