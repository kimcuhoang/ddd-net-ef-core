using FluentValidation;
using System;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail
{
    public class GetCatalogDetailRequestValidator : AbstractValidator<GetCatalogDetailRequest>
    {
        public GetCatalogDetailRequestValidator()
        {
            RuleFor(x => x.CatalogId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual((CatalogId) Guid.Empty);
        }
    }
}
