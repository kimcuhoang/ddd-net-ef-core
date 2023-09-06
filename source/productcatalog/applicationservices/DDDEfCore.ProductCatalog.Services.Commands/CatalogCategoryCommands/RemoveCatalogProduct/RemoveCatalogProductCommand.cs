using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct;

public class RemoveCatalogProductCommand : ITransactionCommand<RemoveCatalogProductResult>
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public CatalogProductId CatalogProductId { get; set; }
}
