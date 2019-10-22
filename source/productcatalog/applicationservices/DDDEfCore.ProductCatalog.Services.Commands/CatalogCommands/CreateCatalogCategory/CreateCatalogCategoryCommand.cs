using System;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory
{
    public class CreateCatalogCategoryCommand : IRequest
    {
        public CatalogId CatalogId { get; }
        public CategoryId CategoryId { get; }
        public CatalogCategoryId ParentCatalogCategoryId { get; }
        public string DisplayName { get;  }

        public CreateCatalogCategoryCommand(Guid catalogId, Guid categoryId, string displayName, Guid? parentCatalogCategoryId = null)
        {
            this.CatalogId = new CatalogId(catalogId);
            this.CategoryId = new CategoryId(categoryId);
            
            this.DisplayName = displayName;

            if (parentCatalogCategoryId.HasValue)
            {
                this.ParentCatalogCategoryId = new CatalogCategoryId(parentCatalogCategoryId.Value);
            }
        }
    }
}
