using DDDEfCore.ProductCatalog.Core.DomainModels.Products;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct;
public class UpdateProductResult
{
    public ProductId ProductId { get; init; } = default!;
}
