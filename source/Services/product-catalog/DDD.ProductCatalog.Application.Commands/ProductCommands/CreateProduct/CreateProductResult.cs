using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Commands.ProductCommands.CreateProduct;
public class CreateProductResult
{
    public ProductId? ProductId { get; init; }
}
