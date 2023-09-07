using DDD.ProductCatalog.Core.Products;
using FluentValidation;

namespace DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductDetail;

public class GetProductDetailRequestValidator : AbstractValidator<GetProductDetailRequest>
{
    public GetProductDetailRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotNull()
            .NotEqual(ProductId.Empty);
    }
}
