using System;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.Core.Common
{
    public interface IRepositoryFactory: IDisposable
    {
        IRepository<TAggregate, TIdentity> CreateRepository<TAggregate, TIdentity>() 
            where TAggregate : AggregateRoot<TIdentity>
            where TIdentity : IdentityBase;
    }
}
