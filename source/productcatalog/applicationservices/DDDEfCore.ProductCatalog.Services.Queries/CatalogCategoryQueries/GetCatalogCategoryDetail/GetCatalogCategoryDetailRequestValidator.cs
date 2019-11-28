using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using System;

namespace DDDEfCore.ProductCatalog.Services.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail
{
    public class GetCatalogCategoryDetailRequestValidator : AbstractValidator<GetCatalogCategoryDetailRequest>
    {
        public GetCatalogCategoryDetailRequestValidator()
        {
            RuleFor(x => x.CatalogCategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual((CatalogCategoryId)Guid.Empty);
        }
    }
}
