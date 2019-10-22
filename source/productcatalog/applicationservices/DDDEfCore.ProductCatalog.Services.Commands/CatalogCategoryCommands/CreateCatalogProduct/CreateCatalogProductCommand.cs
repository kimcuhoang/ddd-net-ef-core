using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.CreateCatalogProduct
{
    public class CreateCatalogProductCommand : IRequest
    {
        public CatalogId CatalogId { get; }
        public CatalogCategoryId CatalogCategoryId { get; }
        public ProductId ProductId { get; }
        public string DisplayName { get; }

        public CreateCatalogProductCommand(Guid catalogId, Guid catalogCategoryId, Guid productId, string displayName)
        {
            this.CatalogId = new CatalogId(catalogId);
            this.CatalogCategoryId = new CatalogCategoryId(catalogCategoryId);
            this.ProductId = new ProductId(productId);
            this.DisplayName = displayName;
        }
    }
}
