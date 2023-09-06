using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogCollections
{
    public class GetCatalogCollectionRequest : IRequest<GetCatalogCollectionResult>
    {
        public string? SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
