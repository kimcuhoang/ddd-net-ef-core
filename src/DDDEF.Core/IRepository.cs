
using System.Linq.Expressions;
using DDDEF.Core.Abstractions;

namespace DDDEF.Core;

public interface IRepository<TAggregationRoot, TAggregationRootId> 
    where TAggregationRootId: IdBase
    where TAggregationRoot: AggregationRoot<TAggregationRootId>
    
{
    IQueryable<TAggregationRoot> GetQueriable();
    Task<List<TAggregationRoot>> Find(Expression<Func<TAggregationRoot, bool>> predicate);
    Task<TAggregationRoot> FindOne(Expression<Func<TAggregationRoot, bool>> predicate);
    void Add(TAggregationRoot aggregationRoot);
}
