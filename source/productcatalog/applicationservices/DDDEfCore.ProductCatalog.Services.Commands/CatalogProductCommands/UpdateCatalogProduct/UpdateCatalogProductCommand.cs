using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogProductCommands.UpdateCatalogProduct;

public class UpdateCatalogProductCommand : IRequest
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public CatalogProductId CatalogProductId { get; set; }
    public string DisplayName { get; set; }
}
