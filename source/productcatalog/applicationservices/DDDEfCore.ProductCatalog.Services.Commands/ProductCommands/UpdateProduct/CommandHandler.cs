using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using FluentValidation;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.UpdateProduct;

public class CommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IRepository<Product, ProductId> _repository;
    private readonly IValidator<UpdateProductCommand> _validator;

    public CommandHandler(IRepository<Product, ProductId> repository, IValidator<UpdateProductCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }

    #region Overrides of IRequestHandler<UpdateProductCommand>

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);
        
        var product = await this._repository.FindOneAsync(x => x.Id == request.ProductId);

        product.ChangeName(request.ProductName);

        //await this._repository.UpdateAsync(product);
    }

    #endregion
}
//TODO: Missing UnitTest