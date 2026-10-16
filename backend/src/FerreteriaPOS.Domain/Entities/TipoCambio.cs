using FerreteriaPOS.Domain.Common;

namespace FerreteriaPOS.Domain.Entities;

public class TipoCambio : Entity
{
    public decimal ValorCompra { get; private set; }
    public decimal ValorVenta { get; private set; }
    public DateTime FechaVigencia { get; private set; }
    public Guid RegistradoPorId { get; private set; }

    // Navigation
    public Usuario RegistradoPor { get; private set; }

    private TipoCambio() { }

    private TipoCambio(
        decimal valorCompra,
        decimal valorVenta,
        Guid registradoPorId,
        DateTime? fechaVigencia = null)
    {
        ValorCompra = valorCompra;
        ValorVenta = valorVenta;
        RegistradoPorId = registradoPorId;
        FechaVigencia = fechaVigencia ?? DateTime.UtcNow;
    }

    public static TipoCambio Crear(
        decimal valorCompra,
        decimal valorVenta,
        Guid registradoPorId,
        DateTime? fechaVigencia = null)
    {
        ValidarValor(valorCompra, nameof(valorCompra));
        ValidarValor(valorVenta, nameof(valorVenta));

        if (valorVenta < valorCompra)
            throw new ArgumentException("El valor de venta no puede ser menor al valor de compra");

        return new TipoCambio(valorCompra, valorVenta, registradoPorId, fechaVigencia);
    }

    public void ActualizarValores(decimal nuevoValorCompra, decimal nuevoValorVenta)
    {
        ValidarValor(nuevoValorCompra, nameof(nuevoValorCompra));
        ValidarValor(nuevoValorVenta, nameof(nuevoValorVenta));

        if (nuevoValorVenta < nuevoValorCompra)
            throw new ArgumentException("El valor de venta no puede ser menor al valor de compra");

        ValorCompra = nuevoValorCompra;
        ValorVenta = nuevoValorVenta;
        FechaVigencia = DateTime.UtcNow;
        ActualizarFechaModificacion();
    }

    private static void ValidarValor(decimal valor, string nombreParametro)
    {
        if (valor <= 0)
            throw new ArgumentException($"El {nombreParametro} debe ser mayor a cero", nombreParametro);
    }
}
