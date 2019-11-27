using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
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
                .NotEqual((CatalogId) Guid.Empty);

            RuleFor(x => x.SearchCatalogCategoryRequest.PageIndex)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0)
                .LessThan(int.MaxValue);

            RuleFor(x => x.SearchCatalogCategoryRequest.PageSize)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0)
                .LessThan(int.MaxValue);
        }
    }
}
