using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;

public class CommandHandler : IRequestHandler<CreateCatalogCommand>
{
    private readonly IRepository<Catalog, CatalogId> _repository;
    private readonly IValidator<CreateCatalogCommand> _validator;

    public CommandHandler(IRepository<Catalog, CatalogId> repository, IValidator<CreateCatalogCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }

    #region Overrides of IRequestHandler<CreateCatalogCommand>

    public async Task Handle(CreateCatalogCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);

        var catalog = Catalog.Create(request.CatalogName);

        foreach (var category in request.Categories)
        {
            catalog.AddCategory(category.CategoryId, category.DisplayName);
        }

        this._repository.Add(catalog);
    }

    #endregion
}
