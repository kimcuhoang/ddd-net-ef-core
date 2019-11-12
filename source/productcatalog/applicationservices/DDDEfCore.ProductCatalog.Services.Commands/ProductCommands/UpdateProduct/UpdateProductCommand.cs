using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct
{
    public class UpdateProductCommand : IRequest
    {
        public ProductId ProductId { get; set; }
        public string ProductName { get; set; }

        public UpdateProductCommand() { }

        public UpdateProductCommand(Guid productId, string productName) : this()
        {
            this.ProductId = new ProductId(productId);
            this.ProductName = productName;
        }
    }
}
