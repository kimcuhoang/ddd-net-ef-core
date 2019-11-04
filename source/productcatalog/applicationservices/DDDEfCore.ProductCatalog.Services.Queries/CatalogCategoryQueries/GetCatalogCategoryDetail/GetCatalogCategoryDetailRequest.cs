using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail
{
    public class GetCatalogCategoryDetailRequest : IRequest<GetCatalogCategoryDetailResult>
    {
        public Guid CatalogCategoryId { get; }
        public CatalogProductSearchRequest CatalogProductCriteria { get; set; } = new CatalogProductSearchRequest();

        public GetCatalogCategoryDetailRequest(Guid catalogCategoryId)
        {
            this.CatalogCategoryId = catalogCategoryId;
        }

        public class CatalogProductSearchRequest
        {
            public string SearchTerm { get; set; }
            public int PageIndex { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }
    }
}
