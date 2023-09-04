using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct;

public class CommandHandler : IRequestHandler<RemoveCatalogProductCommand>
{
    private readonly IRepository<Catalog, CatalogId> _repository;
    private readonly IValidator<RemoveCatalogProductCommand> _validator;

    public CommandHandler(IRepository<Catalog, CatalogId> repository, IValidator<RemoveCatalogProductCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }

    public async Task Handle(RemoveCatalogProductCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);

        var catalogs = this._repository.AsQueryable();

        var query =
            from c in catalogs
            from c1 in c.Categories.Where(_ => _.Id == request.CatalogCategoryId)
            from p in c1.Products.Where(_ => _.Id == request.CatalogProductId)
            where c.Id == request.CatalogId
            select new
            {
                Catalog = c,
                CatalogCategory = c1,
                CatalogProduct = p
            };

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        var catalog = result.Catalog;

        var catalogCategory = result.CatalogCategory;

        var catalogProduct = result.CatalogProduct;

        catalogCategory.RemoveCatalogProduct(catalogProduct);

        await this._repository.UpdateAsync(catalog);
    }
}
