using System;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryDetail
{
    public class GetCategoryDetailRequestValidator : AbstractValidator<GetCategoryDetailRequest>
    {
        public GetCategoryDetailRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual((CategoryId)Guid.Empty);
        }
    }
}
