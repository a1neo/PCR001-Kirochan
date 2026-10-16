using FerreteriaPOS.Domain.Common;

namespace FerreteriaPOS.Domain.Entities;

public class Producto : Entity
{
    public string SKU { get; private set; }
    public string Nombre { get; private set; }
    public string? Descripcion { get; private set; }
    public string? CodigoBarras { get; private set; }
    public Guid CategoriaId { get; private set; }
    public string? Variante { get; private set; }
    public int StockActual { get; private set; }
    public int StockMinimo { get; private set; }
    public string? UnidadMedida { get; private set; }
    public bool PermitirRecalculo { get; private set; }

    // Navigation
    public Categoria Categoria { get; private set; }
    public ICollection<PrecioProducto> Precios { get; private set; }
    public ICollection<MovimientoInventario> Movimientos { get; private set; }
    public FormulaPrecios? FormulaPersonalizada { get; private set; }

    private Producto()
    {
        Precios = new List<PrecioProducto>();
        Movimientos = new List<MovimientoInventario>();
        PermitirRecalculo = true;
    }

    private Producto(
        string sku,
        string nombre,
        Guid categoriaId,
        string? descripcion = null,
        string? codigoBarras = null,
        string? variante = null,
        int stockMinimo = 5,
        string? unidadMedida = null) : this()
    {
        SKU = sku ?? throw new ArgumentNullException(nameof(sku));
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        CategoriaId = categoriaId;
        Descripcion = descripcion;
        CodigoBarras = codigoBarras;
        Variante = variante;
        StockActual = 0;
        StockMinimo = stockMinimo;
        UnidadMedida = unidadMedida;
    }

    public static Producto Crear(
        string sku,
        string nombre,
        Guid categoriaId,
        string? descripcion = null,
        string? codigoBarras = null,
        string? variante = null,
        int stockMinimo = 5,
        string? unidadMedida = null)
    {
        ValidarSKU(sku);
        ValidarNombre(nombre);
        ValidarStockMinimo(stockMinimo);

        return new Producto(sku, nombre, categoriaId, descripcion, codigoBarras, variante, stockMinimo, unidadMedida);
    }

    public void ActualizarInformacion(
        string nombre,
        string? descripcion,
        string? codigoBarras,
        int stockMinimo,
        string? unidadMedida)
    {
        ValidarNombre(nombre);
        ValidarStockMinimo(stockMinimo);

        Nombre = nombre;
        Descripcion = descripcion;
        CodigoBarras = codigoBarras;
        StockMinimo = stockMinimo;
        UnidadMedida = unidadMedida;
        ActualizarFechaModificacion();
    }

    public void AjustarStock(int cantidad, bool esEntrada)
    {
        if (esEntrada)
        {
            StockActual += cantidad;
        }
        else
        {
            if (StockActual < cantidad)
                throw new InvalidOperationException($"Stock insuficiente. Disponible: {StockActual}, Requerido: {cantidad}");

            StockActual -= cantidad;
        }

        ActualizarFechaModificacion();
    }

    public void BloquearRecalculo()
    {
        PermitirRecalculo = false;
        ActualizarFechaModificacion();
    }

    public void DesbloquearRecalculo()
    {
        PermitirRecalculo = true;
        ActualizarFechaModificacion();
    }

    public bool RequiereReabastecimiento()
    {
        return StockActual <= StockMinimo;
    }

    private static void ValidarSKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("El SKU no puede estar vacío", nameof(sku));

        if (sku.Length > 50)
            throw new ArgumentException("El SKU no puede exceder 50 caracteres", nameof(sku));
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto no puede estar vacío", nameof(nombre));

        if (nombre.Length > 200)
            throw new ArgumentException("El nombre del producto no puede exceder 200 caracteres", nameof(nombre));
    }

    private static void ValidarStockMinimo(int stockMinimo)
    {
        if (stockMinimo < 0)
            throw new ArgumentException("El stock mínimo no puede ser negativo", nameof(stockMinimo));
    }
}
