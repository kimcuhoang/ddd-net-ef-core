using DDD.ProductCatalog.Core.Catalogs;
using MediatR;

namespace DDD.ProductCatalog.Application.Commands.CatalogProductCommands.UpdateCatalogProduct;

public class UpdateCatalogProductCommand : IRequest
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public CatalogProductId CatalogProductId { get; set; }
    public string DisplayName { get; set; }
}
