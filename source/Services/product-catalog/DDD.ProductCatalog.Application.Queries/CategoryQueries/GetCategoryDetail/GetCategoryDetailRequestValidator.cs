using DDD.ProductCatalog.Core.Categories;
using FluentValidation;

namespace DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryDetail;

public class GetCategoryDetailRequestValidator : AbstractValidator<GetCategoryDetailRequest>
{
    public GetCategoryDetailRequestValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotNull()
            .NotEqual(CategoryId.Empty);
    }
}
