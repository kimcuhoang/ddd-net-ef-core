using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog;
public class UpdateCatalogResult
{
    public CatalogId CatalogId { get; init; } = default!;

    public bool Success { get; init; } = false;
}
