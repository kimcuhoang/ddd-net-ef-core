using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.CreateCatalogCategory;

public class CreateCatalogCategoryCommand : IProductCatalogCommand<CreateCatalogCategoryResult>
{
    public CatalogId CatalogId { get; set; } = default!;
    public CategoryId CategoryId { get; set; } = default!;
    public CatalogCategoryId? ParentCatalogCategoryId { get; set; }
    public string DisplayName { get; set; } = default!;
}
