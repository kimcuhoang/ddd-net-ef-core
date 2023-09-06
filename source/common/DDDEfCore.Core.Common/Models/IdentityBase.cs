namespace DDDEfCore.Core.Common.Models;

public abstract class IdentityBase : ValueObjectBase
{
    public Guid Id { get; private set; }

    #region Constructors

    protected IdentityBase(Guid id) => this.Id = id;

    #endregion

    #region Overrides of ValueObjectBase

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Id;
    }

    #endregion

    #region Overrides of Object

    public override string ToString() => $"{this.GetType().Name}:{this.Id}";

    #endregion

    public static implicit operator Guid(IdentityBase id) => id.Id;
}
