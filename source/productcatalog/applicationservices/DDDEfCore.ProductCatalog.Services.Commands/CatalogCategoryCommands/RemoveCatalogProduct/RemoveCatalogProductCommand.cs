using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct
{
    public class RemoveCatalogProductCommand : IRequest
    {
        public CatalogId CatalogId { get; set; }
        public CatalogCategoryId CatalogCategoryId { get; set; }
        public CatalogProductId CatalogProductId { get; set; }
    }
}
