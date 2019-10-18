using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.CreateCatalogCategory
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
