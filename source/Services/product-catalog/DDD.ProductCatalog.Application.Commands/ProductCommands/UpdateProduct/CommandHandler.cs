using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Products;
using MediatR;

namespace DDD.ProductCatalog.Application.Commands.ProductCommands.UpdateProduct;

public class CommandHandler(IRepository<Product, ProductId> repository) : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly IRepository<Product, ProductId> _repository = repository;

    public async Task<UpdateProductResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await this._repository.FindOneAsync(x => x.Id == request.ProductId);

        product.ChangeName(request.ProductName);

        return new UpdateProductResult
        {
            ProductId = product.Id
        };
    }
}
//TODO: Missing UnitTest