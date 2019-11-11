using System;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.Core.Common
{
    public interface IRepositoryFactory: IDisposable
    {
        IRepository<TAggregate> CreateRepository<TAggregate>() where TAggregate : AggregateRoot;
    }
}
