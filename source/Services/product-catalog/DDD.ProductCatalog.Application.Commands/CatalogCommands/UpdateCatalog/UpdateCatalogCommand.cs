using DDD.ProductCatalog.Core.Catalogs;
using MediatR;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.UpdateCatalog;

public class UpdateCatalogCommand : IProductCatalogCommand<UpdateCatalogResult>
{
    public CatalogId CatalogId { get; set; }
    public string CatalogName { get; set; }
}
