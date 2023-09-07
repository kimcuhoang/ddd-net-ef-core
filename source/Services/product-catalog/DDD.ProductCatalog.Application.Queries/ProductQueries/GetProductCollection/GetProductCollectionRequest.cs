
namespace DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductCollection;

public class GetProductCollectionRequest : IProductCatalogQueryRequest<GetProductCollectionResult>
{
    public string? SearchTerm { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
