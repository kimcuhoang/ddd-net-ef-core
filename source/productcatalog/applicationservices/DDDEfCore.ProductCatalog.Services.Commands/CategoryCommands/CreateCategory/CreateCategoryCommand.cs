
namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory;

public sealed class CreateCategoryCommand : ITransactionCommand<CreateCategoryResult>
{
    public string CategoryName { get; set; } = default!;
}
