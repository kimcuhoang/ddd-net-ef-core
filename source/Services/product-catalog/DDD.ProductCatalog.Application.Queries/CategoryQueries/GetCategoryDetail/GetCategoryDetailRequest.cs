using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryDetail;

public class GetCategoryDetailRequest : IProductCatalogQueryRequest<GetCategoryDetailResult>
{
    public CategoryId CategoryId { get; set; }
}
