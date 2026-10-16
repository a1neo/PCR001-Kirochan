using FerreteriaPOS.Domain.Common;

namespace FerreteriaPOS.Domain.Entities;

public class Venta : Entity
{
    public string NumeroVenta { get; private set; }
    public DateTime FechaVenta { get; private set; }
    public Guid VendedorId { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Total { get; private set; }
    public Moneda MonedaPago { get; private set; }
    public decimal MontoPagado { get; private set; }
    public decimal Cambio { get; private set; }
    public decimal? TipoCambioAplicado { get; private set; }

    // Navigation
    public Usuario Vendedor { get; private set; }
    private readonly List<DetalleVenta> _detalles = new();
    public IReadOnlyCollection<DetalleVenta> Detalles => _detalles.AsReadOnly();

    private Venta() { }

    private Venta(
        string numeroVenta,
        Guid vendedorId,
        Moneda monedaPago,
        decimal? tipoCambioAplicado = null)
    {
        NumeroVenta = numeroVenta ?? throw new ArgumentNullException(nameof(numeroVenta));
        FechaVenta = DateTime.UtcNow;
        VendedorId = vendedorId;
        MonedaPago = monedaPago;
        TipoCambioAplicado = tipoCambioAplicado;
        Subtotal = 0;
        Total = 0;
        MontoPagado = 0;
        Cambio = 0;
    }

    public static Venta Crear(
        string numeroVenta,
        Guid vendedorId,
        Moneda monedaPago,
        decimal? tipoCambioAplicado = null)
    {
        ValidarNumeroVenta(numeroVenta);
        return new Venta(numeroVenta, vendedorId, monedaPago, tipoCambioAplicado);
    }

    public void AgregarDetalle(
        Guid productoId,
        string productoNombre,
        string productoSKU,
        int cantidad,
        decimal precioUnitario,
        Moneda monedaPrecio)
    {
        ValidarCantidad(cantidad);
        ValidarPrecio(precioUnitario);

        var detalle = DetalleVenta.Crear(
            Id,
            productoId,
            productoNombre,
            productoSKU,
            cantidad,
            precioUnitario,
            monedaPrecio);

        _detalles.Add(detalle);
        RecalcularTotales();
    }

    public void RegistrarPago(decimal montoPagado)
    {
        ValidarPrecio(montoPagado);

        if (montoPagado < Total)
            throw new InvalidOperationException($"El monto pagado ({montoPagado}) es menor al total ({Total})");

        MontoPagado = montoPagado;
        Cambio = montoPagado - Total;
        ActualizarFechaModificacion();
    }

    private void RecalcularTotales()
    {
        Subtotal = _detalles.Sum(d => d.Subtotal);
        Total = Subtotal; // En v1 no hay impuestos ni descuentos
        ActualizarFechaModificacion();
    }

    private static void ValidarNumeroVenta(string numeroVenta)
    {
        if (string.IsNullOrWhiteSpace(numeroVenta))
            throw new ArgumentException("El número de venta no puede estar vacío", nameof(numeroVenta));
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
