using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogProductQueries.GetCatalogProductDetail
{
    public class GetCatalogProductDetailRequest : IRequest<GetCatalogProductDetailResult>
    {
        public Guid CatalogProductId { get; }

        public GetCatalogProductDetailRequest(Guid catalogProductId)
        {
            this.CatalogProductId = catalogProductId;
        }
    }
}
