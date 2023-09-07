using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Commands.CatalogCategoryCommands.CreateCatalogProduct;

public class CreateCatalogProductCommand : IProductCatalogCommand<CreateCatalogProductResult>
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public ProductId ProductId { get; set; }
    public string DisplayName { get; set; }
}
