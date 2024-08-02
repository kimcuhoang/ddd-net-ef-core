using DDDEF.Core.Abstractions;

namespace DDDEF.Core;

public interface IRepositoryFactory
{
    IRepository<TAggregationRoot, TAggregationRootId> GetRepository<TAggregationRoot, TAggregationRootId>()
                    where TAggregationRootId: IdBase
                    where TAggregationRoot: AggregationRoot<TAggregationRootId>;
}
