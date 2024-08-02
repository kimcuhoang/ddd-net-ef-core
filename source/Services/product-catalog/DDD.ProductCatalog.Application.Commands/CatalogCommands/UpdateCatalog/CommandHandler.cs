using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Catalogs;
using MediatR;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.UpdateCatalog;

public class CommandHandler(IRepository<Catalog, CatalogId> repository) : IRequestHandler<UpdateCatalogCommand, UpdateCatalogResult>
{
    private readonly IRepository<Catalog, CatalogId> _repository = repository;

    public async Task<UpdateCatalogResult> Handle(UpdateCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalog = await this._repository.FindOneAsync(x => x.Id == request.CatalogId);

        catalog.ChangeDisplayName(request.CatalogName);

        return new UpdateCatalogResult
        {
            CatalogId = catalog.Id,
            Success = true
        };
    }
}
