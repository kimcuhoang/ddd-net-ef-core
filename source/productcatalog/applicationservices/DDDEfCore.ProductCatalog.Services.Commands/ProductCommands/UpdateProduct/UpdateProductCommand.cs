using DDDEfCore.ProductCatalog.Core.DomainModels.Products;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct;

public class UpdateProductCommand : ITransactionCommand<UpdateProductResult>
{
    public ProductId ProductId { get; set; } = default!;
    public string ProductName { get; set; } = default!;
}
