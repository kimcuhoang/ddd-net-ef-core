using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogProductCommands.UpdateCatalogProduct
{
    public class UpdateCatalogProductCommand : IRequest
    {
        public CatalogId CatalogId { get; }
        public CatalogCategoryId CatalogCategoryId { get; }
        public CatalogProductId CatalogProductId { get; }
        public string DisplayName { get; }

        public UpdateCatalogProductCommand(Guid catalogId, Guid catalogCategoryId, Guid catalogProductId, string displayName)
        {
            this.CatalogId = new CatalogId(catalogId);
            this.CatalogCategoryId = new CatalogCategoryId(catalogCategoryId);
            this.CatalogProductId = new CatalogProductId(catalogProductId);
            this.DisplayName = displayName;
        }

        public UpdateCatalogProductCommand(CatalogId catalogId, CatalogCategoryId catalogCategoryId, CatalogProductId catalogProductId, string displayName)
        {
            this.CatalogId = catalogId;
            this.CatalogCategoryId = catalogCategoryId;
            this.CatalogProductId = catalogProductId;
            this.DisplayName = displayName;
        }
    }
}
