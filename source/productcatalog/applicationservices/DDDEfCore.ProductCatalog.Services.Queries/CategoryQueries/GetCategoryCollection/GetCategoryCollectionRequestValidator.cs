using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryCollection
{
    public class GetCategoryCollectionRequestValidator : AbstractValidator<GetCategoryCollectionRequest>
    {
        public GetCategoryCollectionRequestValidator()
        {
            RuleFor(x => x.PageIndex)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0)
                .LessThan(int.MaxValue);

            RuleFor(x => x.PageSize)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0)
                .LessThan(int.MaxValue);
        }
    }
}
