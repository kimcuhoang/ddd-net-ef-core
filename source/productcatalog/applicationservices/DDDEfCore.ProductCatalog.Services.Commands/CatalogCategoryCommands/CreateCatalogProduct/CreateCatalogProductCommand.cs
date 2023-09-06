using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.CreateCatalogProduct;

public class CreateCatalogProductCommand : ITransactionCommand<CreateCatalogProductResult>
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public ProductId ProductId { get; set; }
    public string DisplayName { get; set; }
}
