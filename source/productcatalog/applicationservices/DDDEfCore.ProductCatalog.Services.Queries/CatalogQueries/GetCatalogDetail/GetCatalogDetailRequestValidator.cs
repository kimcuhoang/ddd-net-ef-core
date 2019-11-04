using FluentValidation;
using System;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail
{
    public class GetCatalogDetailRequestValidator : AbstractValidator<GetCatalogDetailRequest>
    {
        public GetCatalogDetailRequestValidator()
        {
            RuleFor(x => x.CatalogId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual(Guid.Empty);
        }
    }
}
