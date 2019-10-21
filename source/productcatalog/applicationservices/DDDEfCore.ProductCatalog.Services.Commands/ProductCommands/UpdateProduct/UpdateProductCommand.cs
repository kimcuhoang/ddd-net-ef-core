using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct
{
    public class UpdateProductCommand : IRequest
    {
        public ProductId ProductId { get; }
        public string ProductName { get; }

        public UpdateProductCommand(Guid productId, string productName)
        {
            this.ProductId = new ProductId(productId);
            this.ProductName = productName;
        }
    }
}
