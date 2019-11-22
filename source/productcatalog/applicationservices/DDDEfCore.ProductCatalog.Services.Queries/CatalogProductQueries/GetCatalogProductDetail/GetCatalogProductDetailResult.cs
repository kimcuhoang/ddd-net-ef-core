using System;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

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
            public CatalogId CatalogId { get; set; }
            public string CatalogName { get; set; }
        }

        public class CatalogCategoryInfo
        {
            public CatalogCategoryId CatalogCategoryId { get; set; }
            public string DisplayName { get; set; }
        }

        public class CatalogProductInfo
        {
            public CatalogProductId CatalogProductId { get; set; }
            public string DisplayName { get; set; }
            public bool IsNull => this.CatalogProductId == null && string.IsNullOrWhiteSpace(this.DisplayName);
        }
    }
}
