namespace DNK.DDD.Core.Models;

/// <summary>
/// https://enterprisecraftsmanship.com/posts/value-object-better-implementation/
/// </summary>
public abstract class ValueObjectBase
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;

        if (ReferenceEquals(null, obj)) return false;

        if (GetType() != obj.GetType()) return false;

        var valueObject = (ValueObjectBase)obj;

        return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }

    public override int GetHashCode() =>
        GetEqualityComponents().Aggregate(1, (current, obj) =>
        {
            unchecked
            {
                return current * 23 + (obj?.GetHashCode() ?? 0);
            }
        });

    public static bool operator ==(ValueObjectBase object1, ValueObjectBase object2)
    {
        if (ReferenceEquals(object1, null) && ReferenceEquals(object2, null))
            return true;

        if (ReferenceEquals(object1, null) || ReferenceEquals(object2, null))
            return false;

        return object1.Equals(object2);
    }

    public static bool operator !=(ValueObjectBase object1, ValueObjectBase object2)
        => !(object1 == object2);
}
