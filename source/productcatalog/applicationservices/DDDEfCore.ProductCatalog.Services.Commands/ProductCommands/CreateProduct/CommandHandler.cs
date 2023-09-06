using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;

public class CommandHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IRepository<Product, ProductId> _repository;

    public CommandHandler(IRepository<Product, ProductId> repository)
    {
        this._repository = repository;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.ProductName);

        this._repository.Add(product);

        await Task.Yield();

        return new CreateProductResult
        {
            ProductId = product.Id
        };
    }
}
//TODO: Missing UnitTest