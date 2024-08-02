using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Catalogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Application.Commands.CatalogCategoryCommands.RemoveCatalogProduct;

public class CommandHandler(IRepository<Catalog, CatalogId> repository) : IRequestHandler<RemoveCatalogProductCommand, RemoveCatalogProductResult>
{
    private readonly IRepository<Catalog, CatalogId> _repository = repository;

    public async Task<RemoveCatalogProductResult> Handle(RemoveCatalogProductCommand request, CancellationToken cancellationToken)
    {
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

        return RemoveCatalogProductResult.Instance(request);
    }
}
