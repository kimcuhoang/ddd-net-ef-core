using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogProductQueries.GetCatalogProductDetail
{
    public class GetCatalogProductDetailRequest : IRequest<GetCatalogProductDetailResult>
    {
        public CatalogProductId CatalogProductId { get; set; }
    }
}
