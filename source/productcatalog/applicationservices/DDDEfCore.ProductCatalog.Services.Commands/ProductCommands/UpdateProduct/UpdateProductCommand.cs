using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct
{
    public class UpdateProductCommand : IRequest
    {
        public ProductId ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
