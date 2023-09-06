using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryCollection
{
    public class GetCategoryCollectionRequestValidator : AbstractValidator<GetCategoryCollectionRequest>
    {
        public GetCategoryCollectionRequestValidator()
        {
            RuleFor(x => x.PageIndex)
                
                .GreaterThan(0)
                .LessThan(int.MaxValue);

            RuleFor(x => x.PageSize)
                
                .GreaterThan(0)
                .LessThan(int.MaxValue);
        }
    }
}
