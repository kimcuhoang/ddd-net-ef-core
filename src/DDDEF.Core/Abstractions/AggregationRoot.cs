namespace DDDEF.Core.Abstractions;

public abstract class AggregationRoot<AggregationRootId>(AggregationRootId id)
: EntityBase<AggregationRootId>(id) where AggregationRootId : IdBase
{

}
