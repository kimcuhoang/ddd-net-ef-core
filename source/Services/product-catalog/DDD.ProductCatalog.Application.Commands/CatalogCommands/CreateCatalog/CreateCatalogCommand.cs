using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.CreateCatalog;

public class CreateCatalogCommand : IProductCatalogCommand<CreateCatalogResult>
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
        public CategoryId CategoryId { get; set; } = default!;

        public string DisplayName { get; set; } = default!;
    }
}
