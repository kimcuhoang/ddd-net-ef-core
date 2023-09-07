using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.UpdateCatalog;
public class UpdateCatalogResult
{
    public CatalogId CatalogId { get; init; } = default!;

    public bool Success { get; init; } = false;
}
