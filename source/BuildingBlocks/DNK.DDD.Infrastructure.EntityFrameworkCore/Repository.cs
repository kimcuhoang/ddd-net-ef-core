using DNK.DDD.Core;
using DNK.DDD.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DNK.DDD.Infrastructure.EntityFrameworkCore;

public class Repository<TAggregate, TIdentity> : IRepository<TAggregate, TIdentity>
                where TAggregate : AggregateRoot<TIdentity>
                where TIdentity : IdentityBase
{
    private readonly DbContext _dbContext;

    public Repository(DbContext dbContext) => this._dbContext = dbContext;

    #region Implementation of IRepository<TAggregate>

    public IQueryable<TAggregate> AsQueryable()
    {
        return this._dbContext.Set<TAggregate>();
    }

    public async Task<TAggregate?> FindOneAsync(Expression<Func<TAggregate, bool>> predicate)
    {
        return await this._dbContext.Set<TAggregate>().FirstOrDefaultAsync(predicate);
    }

    public void Update(TAggregate aggregate) => this._dbContext.Update(aggregate);

    public void Remove(TAggregate aggregate) => this._dbContext.Remove(aggregate);

    public void Add(TAggregate aggregate) => this._dbContext.Add(aggregate);

    #endregion
}
