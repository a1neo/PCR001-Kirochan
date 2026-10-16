using FerreteriaPOS.Domain.Common;
using System.Text.Json;

namespace FerreteriaPOS.Domain.Entities;

public class FormulaPrecios : Entity
{
    public string Nombre { get; private set; }
    public string DefinicionJson { get; private set; }
    public Guid? CategoriaId { get; private set; }
    public Guid? ProductoId { get; private set; }
    public bool EsGlobal { get; private set; }

    // Navigation
    public Categoria? Categoria { get; private set; }
    public Producto? Producto { get; private set; }

    private FormulaPrecios() { }

    private FormulaPrecios(
        string nombre,
        string definicionJson,
        Guid? categoriaId = null,
        Guid? productoId = null,
        bool esGlobal = false)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        DefinicionJson = definicionJson ?? throw new ArgumentNullException(nameof(definicionJson));
        CategoriaId = categoriaId;
        ProductoId = productoId;
        EsGlobal = esGlobal;

        ValidarAlcance();
    }

    public static FormulaPrecios CrearGlobal(string nombre, string definicionJson)
    {
        ValidarNombre(nombre);
        ValidarJson(definicionJson);
        return new FormulaPrecios(nombre, definicionJson, esGlobal: true);
    }

    public static FormulaPrecios CrearPorCategoria(string nombre, string definicionJson, Guid categoriaId)
    {
        ValidarNombre(nombre);
        ValidarJson(definicionJson);
        return new FormulaPrecios(nombre, definicionJson, categoriaId: categoriaId);
    }

    public static FormulaPrecios CrearPorProducto(string nombre, string definicionJson, Guid productoId)
    {
        ValidarNombre(nombre);
        ValidarJson(definicionJson);
        return new FormulaPrecios(nombre, definicionJson, productoId: productoId);
    }

    public void ActualizarDefinicion(string nuevaDefinicionJson)
    {
        ValidarJson(nuevaDefinicionJson);
        DefinicionJson = nuevaDefinicionJson;
        ActualizarFechaModificacion();
    }

    public void ActualizarNombre(string nuevoNombre)
    {
        ValidarNombre(nuevoNombre);
        Nombre = nuevoNombre;
        ActualizarFechaModificacion();
    }

    private void ValidarAlcance()
    {
        int contadorAlcance = 0;
        if (EsGlobal) contadorAlcance++;
        if (CategoriaId.HasValue) contadorAlcance++;
        if (ProductoId.HasValue) contadorAlcance++;

        if (contadorAlcance != 1)
            throw new InvalidOperationException("La fórmula debe ser global, por categoría o por producto, no múltiples opciones");
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la fórmula no puede estar vacío", nameof(nombre));

        if (nombre.Length > 100)
            throw new ArgumentException("El nombre de la fórmula no puede exceder 100 caracteres", nameof(nombre));
    }

    private static void ValidarJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("La definición JSON no puede estar vacía", nameof(json));

        try
        {
            JsonDocument.Parse(json);
        }
        catch (JsonException)
        {
            throw new ArgumentException("La definición no es un JSON válido", nameof(json));
        }
    }
}
