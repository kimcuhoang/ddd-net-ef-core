﻿using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;

public sealed class CreateCatalogCommand : ITransactionCommand<CreateCatalogResult>
{
    public string CatalogName { get; init; }

    public List<CategoryInCatalog> Categories { get; set; } = new List<CategoryInCatalog>();

    public CreateCatalogCommand AddCategory(CategoryId categoryId, string displayName)
    {
        var category = new CategoryInCatalog
        {
            CategoryId = categoryId,
            DisplayName = displayName
        };
        this.Categories.Add(category);
        return this;
    }

    public class CategoryInCatalog
    {
        public CategoryId CategoryId { get; set; } = default!;

        public string DisplayName { get; set; } = default!;
    }
}
