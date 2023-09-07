﻿using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;

public class GetCatalogCategoryDetailResult
{
    public CatalogCategoryDetailResult CatalogCategoryDetail { get; set; }
    public IEnumerable<CatalogProductResult> CatalogProducts { get; set; }
    public int TotalOfCatalogProducts { get; set; }

    public bool IsNull => this.CatalogCategoryDetail.IsNull;

    public class CatalogCategoryDetailResult
    {
        public CatalogCategoryId CatalogCategoryId { get; set; }
        public string CatalogCategoryName { get; set; }
        public CatalogId CatalogId { get; set; }
        public string CatalogName { get; set; }

        public bool IsNull => this.CatalogCategoryId == null &&
                              string.IsNullOrWhiteSpace(this.CatalogCategoryName);
    }

    public class CatalogProductResult
    {
        public CatalogProductId CatalogProductId { get; set; }
        public string DisplayName { get; set; }
        public ProductId ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
