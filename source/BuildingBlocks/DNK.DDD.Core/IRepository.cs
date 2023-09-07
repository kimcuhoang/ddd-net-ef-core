using DNK.DDD.Core.Models;
using System.Linq.Expressions;

namespace DNK.DDD.Core;

public interface IRepository<TAggregate, TIdentity>
        where TAggregate : AggregateRoot<TIdentity>
        where TIdentity : IdentityBase
{
    IQueryable<TAggregate> AsQueryable();

    Task<TAggregate?> FindOneAsync(Expression<Func<TAggregate, bool>> predicate);

    void Update(TAggregate aggregate);
    void Remove(TAggregate aggregate);
    void Add(TAggregate aggregate);
}
