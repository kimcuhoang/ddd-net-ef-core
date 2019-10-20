using System;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory
{
    public class CreateCatalogCategoryCommand : IRequest
    {
        public Guid CatalogId { get; }
        public Guid CategoryId { get; }
        public Guid? ParentCatalogCategoryId { get; }
        public string DisplayName { get;  }

        public CreateCatalogCategoryCommand(Guid catalogId, Guid categoryId, string displayName, Guid? parentCatalogCategoryId = null)
        {
            this.CatalogId = catalogId;
            this.CategoryId = categoryId;
            this.DisplayName = displayName;
            this.ParentCatalogCategoryId = parentCatalogCategoryId;
        }
    }
}
