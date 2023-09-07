using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryDetail;

public class GetCategoryDetailResult
{
    public CategoryDetailResult CategoryDetail { get; set; }

    public IEnumerable<CatalogOfCategoryResult> AssignedToCatalogs { get; set; }

    public int TotalCatalogs => AssignedToCatalogs?.Count() ?? 0;

    public class CategoryDetailResult
    {
        public CategoryId Id { get; set; }
        public string DisplayName { get; set; }
    }

    public class CatalogOfCategoryResult
    {
        public CatalogId Id { get; set; }
        public string DisplayName { get; set; }
    }
}
