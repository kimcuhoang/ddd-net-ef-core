using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Commands.CategoryCommands.UpdateCategory;

public sealed class UpdateCategoryCommand : IProductCatalogCommand<UpdateCategoryResult>
{
    public CategoryId CategoryId { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
}
