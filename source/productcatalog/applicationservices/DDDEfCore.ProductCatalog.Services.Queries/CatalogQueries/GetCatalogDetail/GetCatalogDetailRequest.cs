using MediatR;
using System;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail
{
    public class GetCatalogDetailRequest : IRequest<GetCatalogDetailResult>
    {
        public Guid CatalogId { get; }

        public CatalogCategorySearchRequest SearchCatalogCategoryRequest { get; set; } = new CatalogCategorySearchRequest();

        public GetCatalogDetailRequest(Guid catalogId)
        {
            this.CatalogId = catalogId;
        }

        public class CatalogCategorySearchRequest
        {
            public string SearchTerm { get; set; }
            public int PageIndex { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }
    }
}
