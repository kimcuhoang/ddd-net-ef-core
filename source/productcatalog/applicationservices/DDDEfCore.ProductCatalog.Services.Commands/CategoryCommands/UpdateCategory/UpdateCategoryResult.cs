using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory;
public class UpdateCategoryResult
{
    public CategoryId CategoryId { get; init; } = default!;
}
