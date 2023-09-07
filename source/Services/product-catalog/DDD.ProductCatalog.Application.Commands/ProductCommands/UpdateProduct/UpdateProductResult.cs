using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Commands.ProductCommands.UpdateProduct;
public class UpdateProductResult
{
    public ProductId ProductId { get; init; } = default!;
}
