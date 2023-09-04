using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;

public sealed class CreateCatalogCommand : IRequest
{
    public string CatalogName { get; set; }

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
        public CategoryId CategoryId { get; set; }

        public string DisplayName { get; set; }
    }
}
