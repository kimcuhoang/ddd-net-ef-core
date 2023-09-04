using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog;

public class CommandHandler : IRequestHandler<UpdateCatalogCommand>
{
    private readonly IRepository<Catalog, CatalogId> _repository;
    private readonly IValidator<UpdateCatalogCommand> _validator;

    public CommandHandler(IRepository<Catalog, CatalogId> repository, IValidator<UpdateCatalogCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }

    #region Overrides of IRequestHandler<UpdateCatalogCommand>

    public async Task Handle(UpdateCatalogCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);

        var catalog = await this._repository.FindOneAsync(x => x.Id == request.CatalogId);

        catalog.ChangeDisplayName(request.CatalogName);

        await this._repository.UpdateAsync(catalog);
    }

    #endregion
}
//TODO: Missing UnitTest