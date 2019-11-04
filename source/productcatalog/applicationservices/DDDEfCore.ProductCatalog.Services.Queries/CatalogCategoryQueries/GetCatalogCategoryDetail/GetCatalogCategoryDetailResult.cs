using System;
using System.Collections.Generic;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail
{
    public class GetCatalogCategoryDetailResult
    {
        public CatalogCategoryDetailResult CatalogCategoryDetail { get; set; }
        public IEnumerable<CatalogProductResult> CatalogProducts { get; set; }
        public int TotalOfCatalogProducts { get; set; }

        public bool IsNull => this.CatalogCategoryDetail.IsNull;

        public class CatalogCategoryDetailResult
        {
            public Guid CatalogCategoryId { get; set; }
            public string CatalogCategoryName { get; set; }
            public Guid CatalogId { get; set; }
            public string CatalogName { get; set; }

            public bool IsNull => this.CatalogCategoryId == Guid.Empty &&
                                  string.IsNullOrWhiteSpace(this.CatalogCategoryName);
        }

        public class CatalogProductResult
        {
            public Guid CatalogProductId { get; set; }
            public string DisplayName { get; set; }
            public Guid ProductId { get; set; }
            public string ProductName { get; set; }
        }
    }
}
