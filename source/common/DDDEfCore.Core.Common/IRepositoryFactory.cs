using System;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.Core.Common
{
    public interface IRepositoryFactory
    {
        IRepository<TAggregate, TIdentity> CreateRepository<TAggregate, TIdentity>() 
            where TAggregate : AggregateRoot<TIdentity>
            where TIdentity : IdentityBase;

        Task Commit(CancellationToken cancellationToken = default);
    }
}
