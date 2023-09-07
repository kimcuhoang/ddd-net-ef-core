using DNK.DDD.Application;

namespace DDD.ProductCatalog.Application.Commands;

public interface IProductCatalogCommand<TResponse> : ITransactionCommand<TResponse> where TResponse : notnull
{

}
