namespace DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryCollection;

public class GetCategoryCollectionRequest : IProductCatalogQueryRequest<GetCategoryCollectionResult>
{
    public string SearchTerm { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
