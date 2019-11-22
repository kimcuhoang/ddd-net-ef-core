using System;
using System.Collections.Generic;

namespace DDDEfCore.Core.Common.Models
{
    public abstract class IdentityBase : ValueObjectBase
    {
        public Guid Id { get; protected set; }

        #region Constructors

        protected IdentityBase(Guid id) => this.Id = id;
        protected IdentityBase() : this(Guid.NewGuid()) { }

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

    public static class IdentityFactory
    {
        public static TIdentity Create<TIdentity>() where TIdentity : IdentityBase
        {
            return Activator.CreateInstance<TIdentity>();
        }
    }
}
