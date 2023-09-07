using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Commands.CategoryCommands.CreateCategory;
public class CreateCategoryResult
{
    public CategoryId? CategoryId { get; init; }
}
