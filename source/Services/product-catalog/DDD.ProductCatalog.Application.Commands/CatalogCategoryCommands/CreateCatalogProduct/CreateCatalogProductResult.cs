using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Commands.CatalogCategoryCommands.CreateCatalogProduct;
public class CreateCatalogProductResult
{
    public CatalogId CatalogId { get; init; }

    public CatalogCategoryId CatalogCategoryId { get; init; }

    public CatalogProductId CatalogProductId { get; init; }

    public static CreateCatalogProductResult Instance(CreateCatalogProductCommand command, CatalogProductId catalogProductId)
    {
        return new CreateCatalogProductResult
        {
            CatalogId = command.CatalogId,
            CatalogCategoryId = command.CatalogCategoryId,
            CatalogProductId = catalogProductId
        };
    }
}
