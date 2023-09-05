
namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;

public class CreateProductCommand : ITransactionCommand<CreateProductResult>
{
    public string ProductName { get; set; } = default!;
}
