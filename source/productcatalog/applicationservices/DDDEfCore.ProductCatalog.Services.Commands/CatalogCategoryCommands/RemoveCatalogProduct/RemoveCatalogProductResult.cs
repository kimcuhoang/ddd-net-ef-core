using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct;

public class RemoveCatalogProductResult
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public CatalogProductId CatalogProductId { get; set; }

    public static RemoveCatalogProductResult Instance(RemoveCatalogProductCommand command)
        => new()
        {
            CatalogId = command.CatalogId,
            CatalogCategoryId = command.CatalogCategoryId,
            CatalogProductId = command.CatalogProductId
        };
}
