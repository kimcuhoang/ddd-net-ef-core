namespace DNK.DDD.Core.Models;

public abstract class EntityBase<TIdentity> : IEquatable<EntityBase<TIdentity>> where TIdentity : IdentityBase
{
    public TIdentity Id { get; private set; }

    #region Constructors

    protected EntityBase(TIdentity id) => this.Id = id;

    #endregion

    #region Implementation of IEquatable<Entity>

    public bool Equals(EntityBase<TIdentity>? other)
    {
        if (ReferenceEquals(null, other)) return false;

        if (ReferenceEquals(this, other)) return true;

        return Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;

        if (ReferenceEquals(this, obj)) return true;

        if (obj.GetType() != this.GetType()) return false;

        return Equals((EntityBase<TIdentity>)obj);
    }

    public override int GetHashCode() => this.GetType().GetHashCode() * 907 + this.Id.GetHashCode();

    #endregion

    #region Overrides of Object

    public override string ToString() => $"{this.GetType().Name}#[Identity={this.Id}]";

    #endregion

    public static bool operator ==(EntityBase<TIdentity> a, EntityBase<TIdentity> b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(EntityBase<TIdentity> a, EntityBase<TIdentity> b)
    {
        return !(a == b);
    }
}
