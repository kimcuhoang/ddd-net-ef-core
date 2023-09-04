using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.RemoveCatalogCategory;

public class CommandHandler : IRequestHandler<RemoveCatalogCategoryCommand>
{
    private readonly IRepository<Catalog, CatalogId> _repository;
    private readonly IValidator<RemoveCatalogCategoryCommand> _validator;

    public CommandHandler(IRepository<Catalog, CatalogId> repository, IValidator<RemoveCatalogCategoryCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }

    #region Overrides of IRequestHandler<RemoveCatalogCategoryCommand>

    public async Task Handle(RemoveCatalogCategoryCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);

        var catalog = await this._repository.FindOneWithIncludeAsync(x => x.Id == request.CatalogId, x => x.Include(c => c.Categories));

        var catalogCategory = catalog.Categories.SingleOrDefault(x => x.Id == request.CatalogCategoryId);

        catalog.RemoveCatalogCategoryWithDescendants(catalogCategory);

        await this._repository.UpdateAsync(catalog);
    }

    #endregion
}
