using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Commands.CatalogProductCommands.UpdateCatalogProduct;
public class UpdateCatalogProductResult
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public CatalogProductId CatalogProductId { get; set; }

    public static UpdateCatalogProductResult Instance(UpdateCatalogProductCommand command)
        => new()
        {
            CatalogId = command.CatalogId,
            CatalogCategoryId = command.CatalogCategoryId,
            CatalogProductId = command.CatalogProductId
        };
}
