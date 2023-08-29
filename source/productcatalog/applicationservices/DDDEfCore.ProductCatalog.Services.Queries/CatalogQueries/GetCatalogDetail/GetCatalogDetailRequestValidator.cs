using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail
{
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
}
