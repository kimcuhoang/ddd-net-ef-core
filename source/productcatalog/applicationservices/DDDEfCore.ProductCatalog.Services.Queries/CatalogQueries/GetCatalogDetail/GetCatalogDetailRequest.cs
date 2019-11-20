using MediatR;
using System;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail
{
    public class GetCatalogDetailRequest : IRequest<GetCatalogDetailResult>
    {
        public CatalogId CatalogId { get; set; }

        public CatalogCategorySearchRequest SearchCatalogCategoryRequest { get; set; } = new CatalogCategorySearchRequest();

        public GetCatalogDetailRequest() { }

        public GetCatalogDetailRequest(Guid catalogId) : this()
        {
            this.CatalogId = (CatalogId)catalogId;
        }

        public class CatalogCategorySearchRequest
        {
            public string SearchTerm { get; set; }
            public int PageIndex { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }
    }
}
