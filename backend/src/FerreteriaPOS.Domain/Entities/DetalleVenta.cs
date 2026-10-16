using FerreteriaPOS.Domain.Common;

namespace FerreteriaPOS.Domain.Entities;

public class DetalleVenta : Entity
{
    public Guid VentaId { get; private set; }
    public Guid ProductoId { get; private set; }
    
    // Snapshot de datos del producto al momento de la venta
    public string ProductoNombre { get; private set; }
    public string ProductoSKU { get; private set; }
    
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public Moneda MonedaPrecio { get; private set; }
    public decimal Subtotal { get; private set; }

    // Navigation
    public Venta Venta { get; private set; }
    public Producto Producto { get; private set; }

    private DetalleVenta() { }

    private DetalleVenta(
        Guid ventaId,
        Guid productoId,
        string productoNombre,
        string productoSKU,
        int cantidad,
        decimal precioUnitario,
        Moneda monedaPrecio)
    {
        VentaId = ventaId;
        ProductoId = productoId;
        ProductoNombre = productoNombre ?? throw new ArgumentNullException(nameof(productoNombre));
        ProductoSKU = productoSKU ?? throw new ArgumentNullException(nameof(productoSKU));
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        MonedaPrecio = monedaPrecio;
        Subtotal = cantidad * precioUnitario;
    }

    public static DetalleVenta Crear(
        Guid ventaId,
        Guid productoId,
        string productoNombre,
        string productoSKU,
        int cantidad,
        decimal precioUnitario,
        Moneda monedaPrecio)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(cantidad));

        if (precioUnitario < 0)
            throw new ArgumentException("El precio unitario no puede ser negativo", nameof(precioUnitario));

        return new DetalleVenta(
            ventaId,
            productoId,
            productoNombre,
            productoSKU,
            cantidad,
            precioUnitario,
            monedaPrecio);
    }
}
