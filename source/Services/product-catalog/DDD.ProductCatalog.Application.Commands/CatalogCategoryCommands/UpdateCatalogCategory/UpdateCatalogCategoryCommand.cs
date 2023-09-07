using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Commands.CatalogCategoryCommands.UpdateCatalogCategory;

public class UpdateCatalogCategoryCommand : IProductCatalogCommand<UpdateCatalogCategoryResult>
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public string DisplayName { get; set; }
}
