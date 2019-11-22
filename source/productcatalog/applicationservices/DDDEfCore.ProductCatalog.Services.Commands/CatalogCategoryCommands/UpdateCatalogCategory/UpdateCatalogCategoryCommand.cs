using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory
{
    public class UpdateCatalogCategoryCommand : IRequest
    {
        public CatalogId CatalogId { get; set; }
        public CatalogCategoryId CatalogCategoryId { get; set; }
        public string DisplayName { get; set; }
    }
}
