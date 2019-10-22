using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct
{
    public class RemoveCatalogProductCommand : IRequest
    {
        public CatalogId CatalogId { get; }
        public CatalogCategoryId CatalogCategoryId { get; set; }
        public CatalogProductId CatalogProductId { get; }

        public RemoveCatalogProductCommand(Guid catalogId, Guid catalogCategoryId, Guid catalogProductId)
        {
            this.CatalogId = new CatalogId(catalogId);
            this.CatalogCategoryId = new CatalogCategoryId(catalogCategoryId);
            this.CatalogProductId = new CatalogProductId(catalogProductId);
        }
    }
}
