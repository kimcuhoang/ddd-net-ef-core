using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogProductQueries.GetCatalogProductDetail
{
    public class GetCatalogProductDetailRequestValidator : AbstractValidator<GetCatalogProductDetailRequest>
    {
        public GetCatalogProductDetailRequestValidator()
        {
            RuleFor(x => x.CatalogProductId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual(CatalogProductId.Empty);
        }
    }
}
