using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.RemoveCatalogCategory;

public class RemoveCatalogCategoryResult
{
    public CatalogId CatalogId { get; init; }
    public CatalogCategoryId CatalogCategoryId { get; init; }

    public static RemoveCatalogCategoryResult Instance(RemoveCatalogCategoryCommand command)
    {
        return new RemoveCatalogCategoryResult
        {
            CatalogId = command.CatalogId,
            CatalogCategoryId = command.CatalogCategoryId,
        };
    }
}
