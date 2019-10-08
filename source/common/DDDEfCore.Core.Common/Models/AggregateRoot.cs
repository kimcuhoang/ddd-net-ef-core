namespace DDDEfCore.Core.Common.Models
{
    public abstract class AggregateRoot : EntityBase
    {
        protected AggregateRoot(IdentityBase id) : base(id)
        {
        }
        protected AggregateRoot() { }
    }
}
