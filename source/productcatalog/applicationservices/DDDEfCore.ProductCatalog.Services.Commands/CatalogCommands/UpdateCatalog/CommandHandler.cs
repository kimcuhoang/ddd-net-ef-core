using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog;

public class CommandHandler : IRequestHandler<UpdateCatalogCommand, UpdateCatalogResult>
{
    private readonly IRepository<Catalog, CatalogId> _repository;

    public CommandHandler(IRepository<Catalog, CatalogId> repository)
    {
        this._repository = repository;
    }

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
