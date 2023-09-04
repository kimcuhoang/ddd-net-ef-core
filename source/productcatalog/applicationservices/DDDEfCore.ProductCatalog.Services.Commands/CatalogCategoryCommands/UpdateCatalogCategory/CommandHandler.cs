using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory;

public class CommandHandler : IRequestHandler<UpdateCatalogCategoryCommand>
{
    private readonly IRepository<Catalog, CatalogId> _repository;
    private readonly IValidator<UpdateCatalogCategoryCommand> _validator;

    public CommandHandler(IRepository<Catalog, CatalogId> repository, IValidator<UpdateCatalogCategoryCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }


    #region Overrides of IRequestHandler<UpdateCatalogCategoryCommand>

    public async Task Handle(UpdateCatalogCategoryCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);

        var catalogs = this._repository.AsQueryable();

        var query =
            from c in catalogs
            from c1 in c.Categories.Where(_ => _.Id == request.CatalogCategoryId).DefaultIfEmpty()
            where c.Id == request.CatalogId
            select new
            {
                Catalog = c,
                CatalogCategory = c1
            };

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        var catalog = result.Catalog;

        var catalogCategory = result.CatalogCategory;

        catalogCategory.ChangeDisplayName(request.DisplayName);

        await this._repository.UpdateAsync(catalog);
    }

    #endregion
}
