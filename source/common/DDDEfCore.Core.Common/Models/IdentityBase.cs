using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DDDEfCore.Core.Common.Models
{
    public abstract class IdentityBase : ValueObjectBase
    {
        public Guid Id { get; protected set; }

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

    public static class IdentityFactory
    {
        public static TIdentity Create<TIdentity>() where TIdentity : IdentityBase
        {
            return Create<TIdentity>(Guid.NewGuid());
        }

        public static TIdentity Create<TIdentity>(object id) where TIdentity : IdentityBase
        {
            if (id == null || (Guid) id == Guid.Empty) return null;

            var identityConstructor = typeof(TIdentity)
                    .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(x => x.GetParameters().Length > 0);

            var instance = identityConstructor?.Invoke(new object[] { id });

            return (TIdentity)instance;
        }
    }
}
