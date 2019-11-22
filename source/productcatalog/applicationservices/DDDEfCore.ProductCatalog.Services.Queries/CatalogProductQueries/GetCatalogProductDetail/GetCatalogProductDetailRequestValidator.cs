using FluentValidation;
using System;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogProductQueries.GetCatalogProductDetail
{
    public class GetCatalogProductDetailRequestValidator : AbstractValidator<GetCatalogProductDetailRequest>
    {
        public GetCatalogProductDetailRequestValidator()
        {
            RuleFor(x => x.CatalogProductId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual((CatalogProductId)Guid.Empty);
        }
    }
}
