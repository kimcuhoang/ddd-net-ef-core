using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

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
            public Guid CategoryId { get; }

            public string DisplayName { get; }

            public CategoryInCatalog(Guid categoryId, string displayName)
            {
                this.CategoryId = categoryId;
                this.DisplayName = displayName;
            }
        }
    }
}
