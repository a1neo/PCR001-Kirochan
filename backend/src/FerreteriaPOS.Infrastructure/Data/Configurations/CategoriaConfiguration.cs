using FerreteriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerreteriaPOS.Infrastructure.Data.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Descripcion)
            .HasMaxLength(500);

        builder.Property(c => c.CategoriaPadreId);

        // Relación auto-referencial
        builder.HasOne(c => c.CategoriaPadre)
            .WithMany(c => c.SubCategorias)
            .HasForeignKey(c => c.CategoriaPadreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.CreadoEn)
            .IsRequired();

        builder.Property(c => c.ModificadoEn);

        builder.Property(c => c.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
