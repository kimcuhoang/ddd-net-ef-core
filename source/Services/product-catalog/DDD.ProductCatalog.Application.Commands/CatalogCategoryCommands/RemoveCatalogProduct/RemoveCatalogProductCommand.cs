using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Commands.CatalogCategoryCommands.RemoveCatalogProduct;

public class RemoveCatalogProductCommand : IProductCatalogCommand<RemoveCatalogProductResult>
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public CatalogProductId CatalogProductId { get; set; }
}
