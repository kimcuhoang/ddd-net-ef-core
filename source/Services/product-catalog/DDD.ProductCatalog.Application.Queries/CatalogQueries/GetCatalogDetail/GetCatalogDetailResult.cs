using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogDetail;

public class GetCatalogDetailResult
{
    public CatalogDetailResult CatalogDetail { get; set; }

    public IEnumerable<CatalogCategorySearchResult> CatalogCategories { get; set; }

    public int TotalOfCatalogCategories { get; set; }

    public bool IsNull => this.CatalogDetail.IsNull;

    public class CatalogDetailResult
    {
        public CatalogId Id { get; set; }
        public string DisplayName { get; set; }

        internal bool IsNull => this.Id == null && string.IsNullOrWhiteSpace(this.DisplayName);
    }

    public class CatalogCategorySearchResult
    {
        public CatalogCategoryId Id { get; set; }
        public CategoryId CategoryId { get; set; }
        public string DisplayName { get; set; }
        public int TotalOfProducts { get; set; }
    }
}
