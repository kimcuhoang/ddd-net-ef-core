using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog
{
    public class UpdateCatalogCommand : IRequest
    {
        public CatalogId CatalogId { get; set; }
        public string CatalogName { get; set; }

        public UpdateCatalogCommand() { }

        public UpdateCatalogCommand(Guid catalogId, string catalogName) : this()
        {
            this.CatalogId = new CatalogId(catalogId);
            this.CatalogName = catalogName;
        }
    }
}
