using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory;
public class CreateCatalogCategoryResult
{
    public CatalogId CatalogId { get; set; } = default!;
    public CatalogCategoryId? ParentCatalogCategoryId { get; set; }

    public CatalogCategoryId CatalogCategoryId { get; init; } = default!;

    public static CreateCatalogCategoryResult Instance(CreateCatalogCategoryCommand command, CatalogCategoryId catalogCategoryId)
    {
        return new CreateCatalogCategoryResult
        {
            CatalogId = command.CatalogId,
            ParentCatalogCategoryId = command.ParentCatalogCategoryId,
            CatalogCategoryId = catalogCategoryId
        };
    }
}
