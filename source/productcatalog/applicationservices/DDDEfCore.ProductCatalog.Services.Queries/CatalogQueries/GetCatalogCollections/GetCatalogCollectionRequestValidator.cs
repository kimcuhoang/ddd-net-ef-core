using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogCollections
{
    public class GetCatalogCollectionRequestValidator : AbstractValidator<GetCatalogCollectionRequest>
    {
        public GetCatalogCollectionRequestValidator()
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
