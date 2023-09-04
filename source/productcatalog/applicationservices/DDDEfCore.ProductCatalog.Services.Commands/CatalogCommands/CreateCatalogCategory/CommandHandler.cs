using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory;

public class CommandHandler : IRequestHandler<CreateCatalogCategoryCommand>
{
    private readonly IRepository<Catalog, CatalogId> _repository;
    private readonly IValidator<CreateCatalogCategoryCommand> _validator;

    public CommandHandler(IRepository<Catalog, CatalogId> repository, IValidator<CreateCatalogCategoryCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }

    #region Overrides of IRequestHandler<CreateCatalogCategoryCommand>

    public async Task Handle(CreateCatalogCategoryCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);

        var catalogs = this._repository.AsQueryable();

        var query =
                from c in catalogs
                from c1 in c.Categories
                        .Where(_ => request.ParentCatalogCategoryId != null && _.Id == request.ParentCatalogCategoryId)
                        .DefaultIfEmpty()
                where c.Id == request.CatalogId
                select new
                {
                    Catalog = c,
                    CatalogCategory = c1
                };

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        var catalog = result.Catalog;

        CatalogCategory parent = result.CatalogCategory;

        catalog.AddCategory(request.CategoryId, request.DisplayName, parent);

        await this._repository.UpdateAsync(catalog);
    }

    #endregion
}
