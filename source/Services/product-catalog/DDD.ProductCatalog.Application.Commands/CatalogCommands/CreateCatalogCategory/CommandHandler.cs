using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Catalogs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.CreateCatalogCategory;

public class CommandHandler : IRequestHandler<CreateCatalogCategoryCommand, CreateCatalogCategoryResult>
{
    private readonly IRepository<Catalog, CatalogId> _repository;

    public CommandHandler(IRepository<Catalog, CatalogId> repository)
    {
        this._repository = repository;
    }

    public async Task<CreateCatalogCategoryResult> Handle(CreateCatalogCategoryCommand request, CancellationToken cancellationToken)
    {
        var catalogs = this._repository.AsQueryable();

        var query =
                from c in catalogs
                let c1 = request.ParentCatalogCategoryId != null
                            ? c.Categories.FirstOrDefault(_ => _.Id == request.ParentCatalogCategoryId)
                            : null
                where c.Id == request.CatalogId
                select new
                {
                    Catalog = c,
                    CatalogCategory = c1
                };

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        var catalog = result.Catalog;

        var catalogCategory = catalog.AddCategory(request.CategoryId, request.DisplayName, result.CatalogCategory);

        return CreateCatalogCategoryResult.Instance(request, catalogCategory.Id);
    }
}
