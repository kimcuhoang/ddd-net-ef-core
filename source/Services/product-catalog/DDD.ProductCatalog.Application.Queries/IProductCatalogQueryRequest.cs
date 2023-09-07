using DNK.DDD.Application;

namespace DDD.ProductCatalog.Application.Queries;

public interface IProductCatalogQueryRequest<TResponse> : IQueryRequest<TResponse> where TResponse : notnull
{
}
