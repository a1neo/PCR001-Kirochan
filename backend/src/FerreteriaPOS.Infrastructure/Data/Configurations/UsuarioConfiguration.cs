using FerreteriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FerreteriaPOS.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.NombreUsuario)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.NombreUsuario)
            .IsUnique();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.NombreCompleto)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.CreadoEn)
            .IsRequired();

        builder.Property(u => u.ModificadoEn);

        builder.Property(u => u.Activo)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
