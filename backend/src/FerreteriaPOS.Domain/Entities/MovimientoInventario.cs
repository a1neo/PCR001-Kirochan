using FerreteriaPOS.Domain.Common;

namespace FerreteriaPOS.Domain.Entities;

public class MovimientoInventario : Entity
{
    public Guid ProductoId { get; private set; }
    public TipoMovimiento TipoMovimiento { get; private set; }
    public int Cantidad { get; private set; }
    public decimal? PrecioUnitario { get; private set; }
    public Moneda? MonedaPrecio { get; private set; }
    public string? Motivo { get; private set; }
    public Guid RegistradoPorId { get; private set; }
    public Guid? VentaId { get; private set; }

    // Navigation
    public Producto Producto { get; private set; }
    public Usuario RegistradoPor { get; private set; }
    public Venta? Venta { get; private set; }

    private MovimientoInventario() { }

    private MovimientoInventario(
        Guid productoId,
        TipoMovimiento tipoMovimiento,
        int cantidad,
        Guid registradoPorId,
        decimal? precioUnitario = null,
        Moneda? monedaPrecio = null,
        string? motivo = null,
        Guid? ventaId = null)
    {
        ProductoId = productoId;
        TipoMovimiento = tipoMovimiento;
        Cantidad = cantidad;
        RegistradoPorId = registradoPorId;
        PrecioUnitario = precioUnitario;
        MonedaPrecio = monedaPrecio;
        Motivo = motivo;
        VentaId = ventaId;
    }

    public static MovimientoInventario Crear(
        Guid productoId,
        TipoMovimiento tipoMovimiento,
        int cantidad,
        Guid registradoPorId,
        decimal? precioUnitario = null,
        Moneda? monedaPrecio = null,
        string? motivo = null,
        Guid? ventaId = null)
    {
        ValidarCantidad(cantidad);

        if (precioUnitario.HasValue)
        {
            ValidarPrecio(precioUnitario.Value);

            if (!monedaPrecio.HasValue)
                throw new ArgumentException("Si se especifica precio, debe especificarse la moneda", nameof(monedaPrecio));
        }

        return new MovimientoInventario(
            productoId,
            tipoMovimiento,
            cantidad,
            registradoPorId,
            precioUnitario,
            monedaPrecio,
            motivo,
            ventaId);
    }

    private static void ValidarCantidad(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(cantidad));
    }

    private static void ValidarPrecio(decimal precio)
    {
        if (precio < 0)
            throw new ArgumentException("El precio no puede ser negativo", nameof(precio));
    }
}
