using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory;

public sealed class UpdateCategoryCommand : ITransactionCommand<UpdateCategoryResult>
{
    public CategoryId CategoryId { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
}
