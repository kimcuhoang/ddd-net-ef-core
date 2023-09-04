using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.RemoveCatalogCategory;

public class RemoveCatalogCategoryCommand : IRequest
{
    public CatalogId CatalogId { get; set; } 
    public CatalogCategoryId CatalogCategoryId { get; set; }
}
