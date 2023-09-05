using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory;

public class CreateCatalogCategoryCommand : ITransactionCommand<CreateCatalogCategoryResult>
{
    public CatalogId CatalogId { get; set; } = default!;
    public CategoryId CategoryId { get; set; } = default!;
    public CatalogCategoryId? ParentCatalogCategoryId { get; set; }
    public string DisplayName { get; set; } = default!;
}
