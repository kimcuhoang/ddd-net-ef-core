using DDD.ProductCatalog.Core.Catalogs;
using MediatR;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.RemoveCatalogCategory;

public class RemoveCatalogCategoryCommand : IProductCatalogCommand<RemoveCatalogCategoryResult>
{
    public CatalogId CatalogId { get; set; }
    public CatalogCategoryId CatalogCategoryId { get; set; }
}
