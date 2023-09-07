
namespace DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogCollections;

public class GetCatalogCollectionRequest : IProductCatalogQueryRequest<GetCatalogCollectionResult>
{
    public string? SearchTerm { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
