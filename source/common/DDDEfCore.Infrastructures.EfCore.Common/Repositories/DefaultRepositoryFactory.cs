using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;

namespace DDDEfCore.Infrastructures.EfCore.Common.Repositories
{
    public class DefaultRepositoryFactory : IRepositoryFactory
    {
        private readonly DbContext _dbContext;

        private ConcurrentDictionary<Type, object> _repositories;

        public DefaultRepositoryFactory(DbContext dbContext)
            => this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        #region Implementation of IRepositoryFactory

        public IRepository<TAggregate, TIdentity> CreateRepository<TAggregate, TIdentity>() 
                        where TAggregate : AggregateRoot<TIdentity>
                        where TIdentity : IdentityBase
        {
            if (_repositories == null) _repositories = new ConcurrentDictionary<Type, object>();

            if (!_repositories.ContainsKey(typeof(TAggregate)))
                _repositories[typeof(TAggregate)] = new DefaultRepositoryAsync<TAggregate, TIdentity>(this._dbContext);

            return (IRepository<TAggregate,TIdentity>)_repositories[typeof(TAggregate)];
        }

        #endregion


        #region Implementation of IDisposable

        public void Dispose()
        {
            this._dbContext?.SaveChanges();
        }

        #endregion
    }
}
