using FluentValidation;

namespace DDD.ProductCatalog.Application.Queries.ProductQueries.GetProductCollection;

public class GetProductCollectionRequestValidator : AbstractValidator<GetProductCollectionRequest>
{
    public GetProductCollectionRequestValidator()
    {
        RuleFor(x => x.PageIndex)

            .GreaterThan(0)
            .LessThan(int.MaxValue);

        RuleFor(x => x.PageSize)

            .GreaterThan(0)
            .LessThan(int.MaxValue);
    }
}
