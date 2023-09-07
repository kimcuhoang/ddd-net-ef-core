namespace DDD.ProductCatalog.Application.Commands.CategoryCommands.CreateCategory;

public sealed class CreateCategoryCommand : IProductCatalogCommand<CreateCategoryResult>
{
    public string CategoryName { get; set; } = default!;
}
