using System;
using System.Collections.Generic;
using System.Text;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.RemoveCatalogCategory
{
    public class RemoveCatalogCategoryCommand : IRequest
    {
        public CatalogId CatalogId { get; } 
        public CatalogCategoryId CatalogCategoryId { get; }

        public RemoveCatalogCategoryCommand(Guid catalogId, Guid catalogCategoryId)
        {
            this.CatalogId = new CatalogId(catalogId);
            this.CatalogCategoryId = new CatalogCategoryId(catalogCategoryId);
        }
    }
}
