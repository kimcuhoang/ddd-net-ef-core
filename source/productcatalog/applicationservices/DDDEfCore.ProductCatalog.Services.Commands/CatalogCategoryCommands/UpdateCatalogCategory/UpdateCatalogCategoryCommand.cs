using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory
{
    public class UpdateCatalogCategoryCommand : IRequest
    {
        public Guid CatalogId { get; set; }
        public Guid CatalogCategoryId { get; }
        public string DisplayName { get; }

        public UpdateCatalogCategoryCommand(Guid catalogId, Guid catalogCategoryId, string displayName)
        {
            this.CatalogId = catalogId;
            this.CatalogCategoryId = catalogCategoryId;
            this.DisplayName = displayName;
        }
    }
}
