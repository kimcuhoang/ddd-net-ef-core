using System;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual(Guid.Empty);

            RuleFor(x => x.CategoryName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().NotEmpty();
        }
    }
}
