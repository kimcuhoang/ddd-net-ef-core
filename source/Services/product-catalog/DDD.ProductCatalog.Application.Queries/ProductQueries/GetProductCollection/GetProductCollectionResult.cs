using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductCollection;

public class GetProductCollectionResult
{
    public int TotalProducts { get; set; }

    public IEnumerable<ProductCollectionItem> Products { get; set; }

    public class ProductCollectionItem
    {
        public ProductId Id { get; set; }
        public string DisplayName { get; set; }
    }
}
