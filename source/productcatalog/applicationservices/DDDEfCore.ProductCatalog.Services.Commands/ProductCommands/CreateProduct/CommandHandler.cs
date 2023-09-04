using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using FluentValidation;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;

public class CommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly IRepository<Product, ProductId> _repository;
    private readonly IValidator<CreateProductCommand> _validator;

    public CommandHandler(IRepository<Product, ProductId> repository, IValidator<CreateProductCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }

    #region Overrides of IRequestHandler<CreateProductCommand>

    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);

        var product = Product.Create(request.ProductName);

        await Task.Yield();

        this._repository.Add(product);
    }

    #endregion
}
//TODO: Missing UnitTest