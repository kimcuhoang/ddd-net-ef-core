using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;

public class CommandHandler : IRequestHandler<CreateCatalogCommand, CreateCatalogResult>
{
    private readonly IRepository<Catalog, CatalogId> _repository;

    public CommandHandler(IRepository<Catalog, CatalogId> repository)
    {
        this._repository = repository;
    }

    public async Task<CreateCatalogResult> Handle(CreateCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalog = Catalog.Create(request.CatalogName);

        foreach (var category in request.Categories)
        {
            catalog.AddCategory(category.CategoryId, category.DisplayName);
        }

        this._repository.Add(catalog);

        await Task.Yield();

        return new CreateCatalogResult { CatalogId = catalog.Id };
    }
}
