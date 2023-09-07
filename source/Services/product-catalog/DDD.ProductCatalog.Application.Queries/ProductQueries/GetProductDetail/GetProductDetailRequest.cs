using DDD.ProductCatalog.Core.Products;
using MediatR;

namespace DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductDetail;

public class GetProductDetailRequest : IRequest<GetProductDetailResult>
{
    public ProductId ProductId { get; set; }
}
