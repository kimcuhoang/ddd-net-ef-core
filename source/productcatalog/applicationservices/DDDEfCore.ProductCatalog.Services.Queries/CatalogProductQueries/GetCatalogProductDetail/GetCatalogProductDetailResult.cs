using System;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogProductQueries.GetCatalogProductDetail
{
    public class GetCatalogProductDetailResult
    {
        public CatalogInfo Catalog { get; set; }
        public CatalogCategoryInfo CatalogCategory { get; set; }
        public CatalogProductInfo CatalogProduct { get; set; }
        public bool IsNull => CatalogProduct.IsNull;

        public class CatalogInfo
        {
            public Guid CatalogId { get; set; }
            public string CatalogName { get; set; }
        }

        public class CatalogCategoryInfo
        {
            public Guid CatalogCategoryId { get; set; }
            public string DisplayName { get; set; }
        }

        public class CatalogProductInfo
        {
            public Guid CatalogProductId { get; set; }
            public string DisplayName { get; set; }
            public bool IsNull => this.CatalogProductId == Guid.Empty && string.IsNullOrWhiteSpace(this.DisplayName);
        }
    }
}
