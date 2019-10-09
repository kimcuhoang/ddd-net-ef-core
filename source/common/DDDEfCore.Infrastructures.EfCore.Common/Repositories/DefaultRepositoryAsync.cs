using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DDDEfCore.Infrastructures.EfCore.Common.Repositories
{
    public class DefaultRepositoryAsync<TAggregate> : IRepository<TAggregate> where TAggregate : AggregateRoot
    {
        private readonly DbContext _dbContext;

        public DefaultRepositoryAsync(DbContext dbContext)
            => this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        #region Implementation of IDisposable

        public void Dispose()
        {
            this._dbContext?.SaveChanges();
        }

        #endregion

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

        #endregion
    }
}
