﻿using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductDetail;

public class GetProductDetailResult
{
    public ProductDetailResult Product { get; set; }

    public IEnumerable<CatalogCategoryResult> CatalogCategories { get; set; }


    public class ProductDetailResult
    {
        public ProductId Id { get; set; }
        public string Name { get; set; }
    }

    public class CatalogCategoryResult
    {
        public CatalogCategoryId CatalogCategoryId { get; set; }
        public string CatalogCategoryName { get; set; }
        public CatalogId CatalogId { get; set; }
        public string CatalogName { get; set; }
        public CatalogProductId CatalogProductId { get; set; }
        public string ProductDisplayName { get; set; }
    }
}
