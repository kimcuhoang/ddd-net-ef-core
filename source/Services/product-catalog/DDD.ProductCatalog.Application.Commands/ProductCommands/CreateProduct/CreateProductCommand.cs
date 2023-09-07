namespace DDD.ProductCatalog.Application.Commands.ProductCommands.CreateProduct;

public class CreateProductCommand : IProductCatalogCommand<CreateProductResult>
{
    public string ProductName { get; set; } = default!;
}
