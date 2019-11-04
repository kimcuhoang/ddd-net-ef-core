using System;
using System.Collections.Generic;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail
{
    public class GetCatalogDetailResult
    {
        public CatalogDetailResult CatalogDetail { get; set; }
        
        public IEnumerable<CatalogCategorySearchResult> CatalogCategories { get; set; }

        public int TotalOfCatalogCategories { get; set; }

        public bool IsNull => this.CatalogDetail.IsNull;

        public class CatalogDetailResult
        {
            public Guid Id { get; set; }
            public string DisplayName { get; set; }

            public bool IsNull => this.Id == Guid.Empty && string.IsNullOrWhiteSpace(this.DisplayName);
        }

        public class CatalogCategorySearchResult
        {
            public Guid CatalogCategoryId { get; set; }
            public Guid CategoryId { get; set; }
            public string DisplayName { get; set; }
            public int TotalOfProducts { get; set; }
        }
    }
}
