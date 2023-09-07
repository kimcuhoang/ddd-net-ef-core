using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Commands.ProductCommands.UpdateProduct;

public class UpdateProductCommand : IProductCatalogCommand<UpdateProductResult>
{
    public ProductId ProductId { get; set; } = default!;
    public string ProductName { get; set; } = default!;
}
