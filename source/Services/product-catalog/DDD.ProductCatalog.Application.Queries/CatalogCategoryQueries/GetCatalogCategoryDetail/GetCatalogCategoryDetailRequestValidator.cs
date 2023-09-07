using DDD.ProductCatalog.Core.Catalogs;
using FluentValidation;

namespace DDD.ProductCatalog.Application.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;

public class GetCatalogCategoryDetailRequestValidator : AbstractValidator<GetCatalogCategoryDetailRequest>
{
    public GetCatalogCategoryDetailRequestValidator()
    {
        RuleFor(x => x.CatalogCategoryId)
            .NotNull()
            .NotEqual(CatalogCategoryId.Empty);
    }
}
