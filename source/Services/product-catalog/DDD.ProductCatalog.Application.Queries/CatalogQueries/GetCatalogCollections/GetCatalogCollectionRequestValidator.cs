using FluentValidation;

namespace DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogCollections;

public class GetCatalogCollectionRequestValidator : AbstractValidator<GetCatalogCollectionRequest>
{
    public GetCatalogCollectionRequestValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0)
            .LessThan(int.MaxValue);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThan(int.MaxValue);
    }
}
