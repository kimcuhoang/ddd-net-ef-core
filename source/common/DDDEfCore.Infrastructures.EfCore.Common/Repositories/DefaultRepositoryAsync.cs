using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DDDEfCore.Infrastructures.EfCore.Common.Repositories;

public class DefaultRepositoryAsync<TAggregate, TIdentity> : IRepository<TAggregate, TIdentity> 
                where TAggregate : AggregateRoot<TIdentity>
                where TIdentity : IdentityBase
{
    private readonly DbContext _dbContext;

    public DefaultRepositoryAsync(DbContext dbContext)
        => this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    #region Implementation of IRepository<TAggregate>

    public IQueryable<TAggregate> AsQueryable()
    {
        return this._dbContext.Set<TAggregate>();
    }

    public async Task<TAggregate> FindOneAsync(Expression<Func<TAggregate, bool>> predicate)
    {
        return await this._dbContext.Set<TAggregate>().FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(TAggregate aggregate)
    {
        await this._dbContext.AddAsync(aggregate);
    }

    public Task UpdateAsync(TAggregate aggregate)
    {
        return Task.FromResult(this._dbContext.Update(aggregate));
    }

    public Task RemoveAsync(TAggregate aggregate)
    {
        return Task.FromResult(this._dbContext.Remove(aggregate));
    }

    public void Add(TAggregate aggregate) => this._dbContext.Add(aggregate);

    #endregion
}
