using FerreteriaPOS.Domain.Common;

namespace FerreteriaPOS.Domain.Entities;

public class PrecioProducto : Entity
{
    public Guid ProductoId { get; private set; }
    public TipoPrecio TipoPrecio { get; private set; }
    public decimal Valor { get; private set; }
    public Moneda Moneda { get; private set; }
    public DateTime FechaVigencia { get; private set; }

    // Navigation
    public Producto Producto { get; private set; }

    private PrecioProducto() { }

    private PrecioProducto(
        Guid productoId,
        TipoPrecio tipoPrecio,
        decimal valor,
        Moneda moneda,
        DateTime? fechaVigencia = null)
    {
        ProductoId = productoId;
        TipoPrecio = tipoPrecio;
        Valor = valor;
        Moneda = moneda;
        FechaVigencia = fechaVigencia ?? DateTime.UtcNow;
    }

    public static PrecioProducto Crear(
        Guid productoId,
        TipoPrecio tipoPrecio,
        decimal valor,
        Moneda moneda,
        DateTime? fechaVigencia = null)
    {
        ValidarValor(valor);
        return new PrecioProducto(productoId, tipoPrecio, valor, moneda, fechaVigencia);
    }

    public void ActualizarValor(decimal nuevoValor)
    {
        ValidarValor(nuevoValor);
        Valor = nuevoValor;
        FechaVigencia = DateTime.UtcNow;
        ActualizarFechaModificacion();
    }

    public decimal ConvertirA(Moneda monedaDestino, decimal tipoCambio)
    {
        if (Moneda == monedaDestino)
            return Valor;

        // Si es USD a BOB
        if (Moneda == Moneda.USD && monedaDestino == Moneda.BOB)
            return Valor * tipoCambio;

        // Si es BOB a USD
        if (Moneda == Moneda.BOB && monedaDestino == Moneda.USD)
            return Valor / tipoCambio;

        throw new InvalidOperationException($"Conversión no soportada de {Moneda} a {monedaDestino}");
    }

    private static void ValidarValor(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("El valor del precio no puede ser negativo", nameof(valor));
    }
}
