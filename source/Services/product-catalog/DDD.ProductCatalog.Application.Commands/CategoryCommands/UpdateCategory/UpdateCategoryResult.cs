using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Commands.CategoryCommands.UpdateCategory;
public class UpdateCategoryResult
{
    public CategoryId CategoryId { get; init; } = default!;
}
