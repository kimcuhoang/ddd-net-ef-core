using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory;

public class UpdateCatalogCategoryCommand : ITransactionCommand<UpdateCatalogCategoryResult>
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public string DisplayName { get; set; }
}
