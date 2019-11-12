using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;
using System;
using System.Collections.Generic;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog
{
    public sealed class CreateCatalogCommand : IRequest
    {
        public string CatalogName { get; set; }

        public List<CategoryInCatalog> Categories { get; set; } = new List<CategoryInCatalog>();

        public CreateCatalogCommand() { }

        public CreateCatalogCommand(string catalogName) : this()
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
            public CategoryId CategoryId { get; set; }

            public string DisplayName { get; set; }

            public CategoryInCatalog() { }

            public CategoryInCatalog(Guid categoryId, string displayName) : this()
            {
                this.CategoryId = new CategoryId(categoryId);
                this.DisplayName = displayName;
            }
        }
    }
}
