namespace FerreteriaPOS.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreadoEn { get; protected set; }
    public DateTime? ModificadoEn { get; protected set; }
    public bool Activo { get; protected set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        CreadoEn = DateTime.UtcNow;
        Activo = true;
    }

    protected Entity(Guid id) : this()
    {
        Id = id;
    }

    public void Desactivar()
    {
        Activo = false;
        ModificadoEn = DateTime.UtcNow;
    }

    public void Reactivar()
    {
        Activo = true;
        ModificadoEn = DateTime.UtcNow;
    }

    protected void ActualizarFechaModificacion()
    {
        ModificadoEn = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}
