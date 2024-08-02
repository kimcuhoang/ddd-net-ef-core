using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Catalogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Application.Commands.CatalogCategoryCommands.CreateCatalogProduct;

public class CommandHandler(IRepository<Catalog, CatalogId> repository) : IRequestHandler<CreateCatalogProductCommand, CreateCatalogProductResult>
{
    private readonly IRepository<Catalog, CatalogId> _repository = repository;

    public async Task<CreateCatalogProductResult> Handle(CreateCatalogProductCommand request, CancellationToken cancellationToken)
    {
        var catalogs = this._repository.AsQueryable();

        var query =
            from c in catalogs
            from c1 in c.Categories.Where(_ => _.Id == request.CatalogCategoryId)
            where c.Id == request.CatalogId
            select new
            {
                Catalog = c,
                CatalogCategory = c1
            };

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        var catalog = result.Catalog;

        var catalogCategory = result.CatalogCategory;

        var catalogProduct = catalogCategory.CreateCatalogProduct(request.ProductId, request.DisplayName);

        return CreateCatalogProductResult.Instance(request, catalogProduct.Id);
    }
}
