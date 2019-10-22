using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;
using System;
using System.Collections.Generic;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog
{
    public sealed class CreateCatalogCommand : IRequest
    {
        public string CatalogName { get; }

        public List<CategoryInCatalog> Categories { get; set; } = new List<CategoryInCatalog>();

        public CreateCatalogCommand(string catalogName)
        {
            this.CatalogName = catalogName;
        }

        public CreateCatalogCommand AddCategory(Guid categoryId, string displayName)
        {
            var category = new CategoryInCatalog(categoryId, displayName);
            this.Categories.Add(category);
            return this;
        }

        public class CategoryInCatalog
        {
            public CategoryId CategoryId { get; }

            public string DisplayName { get; }

            public CategoryInCatalog(Guid categoryId, string displayName)
            {
                this.CategoryId = new CategoryId(categoryId);
                this.DisplayName = displayName;
            }
        }
    }
}
