using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryCollection
{
    public class GetCategoryCollectionRequest : IRequest<GetCategoryCollectionResult>
    {
        public string SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
