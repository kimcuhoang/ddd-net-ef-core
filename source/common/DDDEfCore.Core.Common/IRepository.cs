using DDDEfCore.Core.Common.Models;
using System.Linq.Expressions;

namespace DDDEfCore.Core.Common;

public interface IRepository<TAggregate, TIdentity> 
        where TAggregate : AggregateRoot<TIdentity>
        where TIdentity : IdentityBase
{
    IQueryable<TAggregate> AsQueryable();
    Task<TAggregate> FindOneAsync(Expression<Func<TAggregate, bool>> predicate);
    Task AddAsync(TAggregate aggregate);
    Task UpdateAsync(TAggregate aggregate);
    Task RemoveAsync(TAggregate aggregate);
}
