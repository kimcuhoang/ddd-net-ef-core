using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.Core.Common
{
    public interface IRepositoryFactory
    {
        IRepository<TAggregate> CreateRepository<TAggregate>() where TAggregate : AggregateRoot;
    }
}
