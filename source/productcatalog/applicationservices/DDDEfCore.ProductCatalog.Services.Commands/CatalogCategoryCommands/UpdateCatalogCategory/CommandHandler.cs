using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory;

public class CommandHandler : IRequestHandler<UpdateCatalogCategoryCommand, UpdateCatalogCategoryResult>
{
    private readonly IRepository<Catalog, CatalogId> _repository;

    public CommandHandler(IRepository<Catalog, CatalogId> repository)
    {
        this._repository = repository;
    }


    #region Overrides of IRequestHandler<UpdateCatalogCategoryCommand>

    public async Task<UpdateCatalogCategoryResult> Handle(UpdateCatalogCategoryCommand request, CancellationToken cancellationToken)
    {
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

        return UpdateCatalogCategoryResult.Instance(request);
    }

    #endregion
}
