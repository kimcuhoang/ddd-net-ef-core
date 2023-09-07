using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;

public class GetCatalogCategoryDetailRequest : IProductCatalogQueryRequest<GetCatalogCategoryDetailResult>
{
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public CatalogProductSearchRequest CatalogProductCriteria { get; set; } = new CatalogProductSearchRequest();

    public class CatalogProductSearchRequest
    {
        public string SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
