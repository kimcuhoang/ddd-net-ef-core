using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogCollections;

public class GetCatalogCollectionResult
{
    public int TotalCatalogs { get; set; }

    public IEnumerable<CatalogItem> CatalogItems { get; set; }

    public class CatalogItem
    {
        public CatalogId CatalogId { get; set; }
        public string DisplayName { get; set; }
        public int TotalCategories { get; set; }
    }
}
