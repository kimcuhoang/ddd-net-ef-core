using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using System.Collections.Generic;

namespace DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductCollection
{
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
}
