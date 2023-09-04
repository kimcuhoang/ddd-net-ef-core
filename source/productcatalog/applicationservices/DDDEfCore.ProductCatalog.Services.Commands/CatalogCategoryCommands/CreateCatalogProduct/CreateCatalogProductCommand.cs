using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.CreateCatalogProduct;

public class CreateCatalogProductCommand : IRequest
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
    public ProductId ProductId { get; set; }
    public string DisplayName { get; set; }

    //public CreateCatalogProductCommand(Guid catalogId, Guid catalogCategoryId, Guid productId, string displayName)
    //{
    //    this.CatalogId = new CatalogId(catalogId);
    //    this.CatalogCategoryId = new CatalogCategoryId(catalogCategoryId);
    //    this.ProductId = new ProductId(productId);
    //    this.DisplayName = displayName;
    //}
}
