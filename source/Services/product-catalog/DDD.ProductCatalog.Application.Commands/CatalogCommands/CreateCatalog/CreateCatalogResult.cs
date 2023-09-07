using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.CreateCatalog;

public class CreateCatalogResult
{
    public CatalogId? CatalogId { get; init; }
}
