using MediatR;
using System;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory
{
    public class UpdateCatalogCategoryCommand : IRequest
    {
        public CatalogId CatalogId { get; set; }
        public CatalogCategoryId CatalogCategoryId { get; }
        public string DisplayName { get; }

        public UpdateCatalogCategoryCommand(Guid catalogId, Guid catalogCategoryId, string displayName)
        {
            this.CatalogId = new CatalogId(catalogId);
            this.CatalogCategoryId = new CatalogCategoryId(catalogCategoryId);
            this.DisplayName = displayName;
        }
    }
}
