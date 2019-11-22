using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog
{
    public class UpdateCatalogCommand : IRequest
    {
        public CatalogId CatalogId { get; set; }
        public string CatalogName { get; set; }
    }
}
