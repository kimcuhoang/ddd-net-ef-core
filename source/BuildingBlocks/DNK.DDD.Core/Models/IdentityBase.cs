﻿namespace DNK.DDD.Core.Models;

public abstract class IdentityBase(Guid id) : ValueObjectBase
{
    public Guid Id { get; private set; } = id;

    #region Constructors

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
