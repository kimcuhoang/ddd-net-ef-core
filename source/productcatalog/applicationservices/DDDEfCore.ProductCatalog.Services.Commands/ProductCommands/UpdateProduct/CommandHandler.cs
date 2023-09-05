using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct;

public class CommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly IRepository<Product, ProductId> _repository;

    public CommandHandler(IRepository<Product, ProductId> repository)
    {
        this._repository = repository;
    }
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