using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail
{
    public class GetCatalogCategoryDetailRequest : IRequest<GetCatalogCategoryDetailResult>
    {
        public CatalogCategoryId CatalogCategoryId { get; set; }
        public CatalogProductSearchRequest CatalogProductCriteria { get; set; } = new CatalogProductSearchRequest();

        public class CatalogProductSearchRequest
        {
            public string SearchTerm { get; set; }
            public int PageIndex { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }
    }
}
