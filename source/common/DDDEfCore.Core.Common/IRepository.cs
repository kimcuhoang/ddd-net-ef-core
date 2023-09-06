using DDDEfCore.Core.Common.Models;
using System.Linq.Expressions;

namespace DDDEfCore.Core.Common;

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
