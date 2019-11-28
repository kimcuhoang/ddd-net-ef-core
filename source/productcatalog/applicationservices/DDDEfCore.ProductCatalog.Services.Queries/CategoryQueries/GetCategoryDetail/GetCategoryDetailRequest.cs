using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryDetail
{
    public class GetCategoryDetailRequest : IRequest<GetCategoryDetailResult>
    {
        public CategoryId CategoryId { get; set; }
    }
}
