using DDD.ProductCatalog.Core.Catalogs;
using FluentValidation;

namespace DDD.ProductCatalog.Application.Queries.CatalogQueries.GetCatalogDetail;

public class GetCatalogDetailRequestValidator : AbstractValidator<GetCatalogDetailRequest>
{
    public GetCatalogDetailRequestValidator()
    {
        RuleFor(x => x.CatalogId)
            .NotNull()
            .NotEqual(CatalogId.Empty);

        RuleFor(x => x.SearchCatalogCategoryRequest.PageIndex)
            .GreaterThan(0)
            .LessThan(int.MaxValue);

        RuleFor(x => x.SearchCatalogCategoryRequest.PageSize)
            .GreaterThan(0)
            .LessThan(int.MaxValue);
    }
}
