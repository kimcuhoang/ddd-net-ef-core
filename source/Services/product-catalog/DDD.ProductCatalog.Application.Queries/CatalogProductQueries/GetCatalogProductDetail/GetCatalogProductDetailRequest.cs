using DDD.ProductCatalog.Core.Catalogs;

namespace DDD.ProductCatalog.Application.Queries.CatalogProductQueries.GetCatalogProductDetail
{
    public class GetCatalogProductDetailRequest : IProductCatalogQueryRequest<GetCatalogProductDetailResult>
    {
        public CatalogProductId CatalogProductId { get; set; }
    }
}
