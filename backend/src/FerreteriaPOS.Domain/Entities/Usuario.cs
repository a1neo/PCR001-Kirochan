using FerreteriaPOS.Domain.Common;

namespace FerreteriaPOS.Domain.Entities;

public class Usuario : Entity
{
    public string NombreUsuario { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string NombreCompleto { get; private set; }
    public Rol Rol { get; private set; }

    private Usuario() { }

    private Usuario(
        string nombreUsuario,
        string email,
        string passwordHash,
        string nombreCompleto,
        Rol rol)
    {
        NombreUsuario = nombreUsuario ?? throw new ArgumentNullException(nameof(nombreUsuario));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        NombreCompleto = nombreCompleto ?? throw new ArgumentNullException(nameof(nombreCompleto));
        Rol = rol;
    }

    public static Usuario Crear(
        string nombreUsuario,
        string email,
        string passwordHash,
        string nombreCompleto,
        Rol rol)
    {
        ValidarNombreUsuario(nombreUsuario);
        ValidarEmail(email);
        ValidarNombreCompleto(nombreCompleto);

        return new Usuario(nombreUsuario, email, passwordHash, nombreCompleto, rol);
    }

    public void ActualizarPerfil(string email, string nombreCompleto)
    {
        ValidarEmail(email);
        ValidarNombreCompleto(nombreCompleto);

        Email = email;
        NombreCompleto = nombreCompleto;
        ActualizarFechaModificacion();
    }

    public void CambiarPassword(string nuevoPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(nuevoPasswordHash))
            throw new ArgumentException("El hash del password no puede estar vacío", nameof(nuevoPasswordHash));

        PasswordHash = nuevoPasswordHash;
        ActualizarFechaModificacion();
    }

    public void CambiarRol(Rol nuevoRol)
    {
        Rol = nuevoRol;
        ActualizarFechaModificacion();
    }

    private static void ValidarNombreUsuario(string nombreUsuario)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario))
            throw new ArgumentException("El nombre de usuario no puede estar vacío", nameof(nombreUsuario));

        if (nombreUsuario.Length < 3 || nombreUsuario.Length > 50)
            throw new ArgumentException("El nombre de usuario debe tener entre 3 y 50 caracteres", nameof(nombreUsuario));
    }

    private static void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío", nameof(email));

        if (!email.Contains('@'))
            throw new ArgumentException("El email no tiene un formato válido", nameof(email));
    }

    private static void ValidarNombreCompleto(string nombreCompleto)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new ArgumentException("El nombre completo no puede estar vacío", nameof(nombreCompleto));

        if (nombreCompleto.Length < 3 || nombreCompleto.Length > 100)
            throw new ArgumentException("El nombre completo debe tener entre 3 y 100 caracteres", nameof(nombreCompleto));
    }
}
