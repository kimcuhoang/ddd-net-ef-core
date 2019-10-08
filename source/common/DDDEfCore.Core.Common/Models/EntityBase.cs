using System;

namespace DDDEfCore.Core.Common.Models
{
    public abstract class EntityBase : IEquatable<EntityBase>
    {
        protected IdentityBase Id;

        #region Constructors

        protected EntityBase(IdentityBase id) => this.Id = id;

        protected EntityBase() { }

        #endregion

        #region Implementation of IEquatable<Entity>

        public bool Equals(EntityBase other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != this.GetType()) return false;

            return Equals((EntityBase)obj);
        }

        public override int GetHashCode() => this.GetType().GetHashCode() * 907 + this.Id.GetHashCode();

        #endregion

        #region Overrides of Object

        public override string ToString() => $"{this.GetType().Name}#[Identity={this.Id}]";

        #endregion

        public static bool operator ==(EntityBase a, EntityBase b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EntityBase a, EntityBase b)
        {
            return !(a == b);
        }
    }
}
