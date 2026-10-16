using FerreteriaPOS.Domain.Common;

namespace FerreteriaPOS.Domain.Entities;

public class Categoria : Entity
{
    public string Nombre { get; private set; }
    public string? Descripcion { get; private set; }
    public Guid? CategoriaPadreId { get; private set; }
    
    // Navigation
    public Categoria? CategoriaPadre { get; private set; }
    public ICollection<Categoria> SubCategorias { get; private set; }
    public ICollection<Producto> Productos { get; private set; }
    public ICollection<FormulaPrecios> FormulasPrecios { get; private set; }

    private Categoria()
    {
        SubCategorias = new List<Categoria>();
        Productos = new List<Producto>();
        FormulasPrecios = new List<FormulaPrecios>();
    }

    private Categoria(string nombre, string? descripcion, Guid? categoriaPadreId) : this()
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Descripcion = descripcion;
        CategoriaPadreId = categoriaPadreId;
    }

    public static Categoria Crear(string nombre, string? descripcion = null, Guid? categoriaPadreId = null)
    {
        ValidarNombre(nombre);
        return new Categoria(nombre, descripcion, categoriaPadreId);
    }

    public void ActualizarInformacion(string nombre, string? descripcion)
    {
        ValidarNombre(nombre);
        Nombre = nombre;
        Descripcion = descripcion;
        ActualizarFechaModificacion();
    }

    public void AsignarCategoriaPadre(Guid? categoriaPadreId)
    {
        CategoriaPadreId = categoriaPadreId;
        ActualizarFechaModificacion();
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío", nameof(nombre));

        if (nombre.Length > 100)
            throw new ArgumentException("El nombre de la categoría no puede exceder 100 caracteres", nameof(nombre));
    }
}
