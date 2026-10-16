using FerreteriaPOS.Domain.Common;
using FerreteriaPOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FerreteriaPOS.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedUsuariosAsync(context);
        await SeedCategoriasAsync(context);
    }

    private static async Task SeedUsuariosAsync(ApplicationDbContext context)
    {
        if (await context.Usuarios.AnyAsync())
            return;

        // Password: "Admin123!" hasheado con BCrypt (placeholder - en producción usar BCrypt real)
        // Este hash corresponde a "Admin123!" para desarrollo/demo
        var passwordHash = "$2a$11$rBkUBjIOIL9jzIqN2rN7R.yCfbDFZr4SaLg7NOqkU9HKdvJpIbXEK";

        var superAdmin = Usuario.Crear(
            nombreUsuario: "superadmin",
            email: "superadmin@ferreteriaCS.com",
            passwordHash: passwordHash,
            nombreCompleto: "Super Administrador",
            rol: Rol.SuperAdministrador
        );

        var admin = Usuario.Crear(
            nombreUsuario: "admin",
            email: "admin@ferreteriaCS.com",
            passwordHash: passwordHash,
            nombreCompleto: "Administrador Principal",
            rol: Rol.Administrador
        );

        var vendedor = Usuario.Crear(
            nombreUsuario: "vendedor1",
            email: "vendedor1@ferreteriaCS.com",
            passwordHash: passwordHash,
            nombreCompleto: "Vendedor Uno",
            rol: Rol.Vendedor
        );

        context.Usuarios.AddRange(superAdmin, admin, vendedor);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Usuarios iniciales creados");
    }

    private static async Task SeedCategoriasAsync(ApplicationDbContext context)
    {
        if (await context.Categorias.AnyAsync())
            return;

        // Categorías principales
        var herramientas = Categoria.Crear("Herramientas", "Herramientas manuales y eléctricas");
        var materialesElectricos = Categoria.Crear("Materiales Eléctricos", "Cables, interruptores y accesorios");
        var plomeria = Categoria.Crear("Plomería", "Tuberías, llaves y accesorios de agua");
        var pinturas = Categoria.Crear("Pinturas y Acabados", "Pinturas, barnices y accesorios");
        var fijacion = Categoria.Crear("Fijación y Tornillería", "Tornillos, clavos, pernos y anclas");
        var construccion = Categoria.Crear("Materiales de Construcción", "Cemento, arena, ladrillos");

        context.Categorias.AddRange(
            herramientas, materialesElectricos, plomeria,
            pinturas, fijacion, construccion
        );
        await context.SaveChangesAsync();

        // Subcategorías de Herramientas
        var herramientasMano = Categoria.Crear("Herramientas de Mano", "Martillos, destornilladores, llaves", herramientas.Id);
        var herramientasElectricas = Categoria.Crear("Herramientas Eléctricas", "Taladros, sierras, amoladoras", herramientas.Id);
        var medicion = Categoria.Crear("Medición y Trazado", "Cintas métricas, niveles, escuadras", herramientas.Id);

        // Subcategorías de Materiales Eléctricos
        var cables = Categoria.Crear("Cables y Conductores", "Cables de distintos calibres", materialesElectricos.Id);
        var tomacorrientes = Categoria.Crear("Tomacorrientes e Interruptores", "Tomas, switches y placas", materialesElectricos.Id);
        var iluminacion = Categoria.Crear("Iluminación", "Focos, tubos y accesorios", materialesElectricos.Id);

        context.Categorias.AddRange(
            herramientasMano, herramientasElectricas, medicion,
            cables, tomacorrientes, iluminacion
        );
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Categorías iniciales creadas");
    }
}
