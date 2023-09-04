using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;

public class CreateProductCommand : IRequest
{
    public string ProductName { get; set; }
}
