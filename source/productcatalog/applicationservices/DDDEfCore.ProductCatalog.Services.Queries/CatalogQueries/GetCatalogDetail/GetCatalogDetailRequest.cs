using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail;

public class GetCatalogDetailRequest : IRequest<GetCatalogDetailResult>
{
    public CatalogId CatalogId { get; set; }

    public CatalogCategorySearchRequest SearchCatalogCategoryRequest { get; set; } = new CatalogCategorySearchRequest();

    public class CatalogCategorySearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
