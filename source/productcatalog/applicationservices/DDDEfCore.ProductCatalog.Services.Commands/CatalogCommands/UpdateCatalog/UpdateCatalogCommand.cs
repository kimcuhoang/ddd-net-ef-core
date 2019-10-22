using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog
{
    public class UpdateCatalogCommand : IRequest
    {
        public CatalogId CatalogId { get; }
        public string CatalogName { get; }

        public UpdateCatalogCommand(Guid catalogId, string catalogName)
        {
            this.CatalogId = new CatalogId(catalogId);
            this.CatalogName = catalogName;
        }
    }
}
