using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory;

public class CreateCatalogCategoryCommand : IRequest
{
    public CatalogId CatalogId { get; set; }
    public CategoryId CategoryId { get; set; }
    public CatalogCategoryId ParentCatalogCategoryId { get; set; }
    public string DisplayName { get; set; }
}
