namespace DNK.DDD.Core.Models;

public abstract class AggregateRoot<TIdentity>(TIdentity id) : EntityBase<TIdentity>(id) where TIdentity : IdentityBase
{
}
