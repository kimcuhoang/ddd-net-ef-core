using FluentValidation;
using System;
using System.Linq;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog
{
    public class CreateCatalogCommandValidator : AbstractValidator<CreateCatalogCommand>
    {
        public CreateCatalogCommandValidator()
        {
            RuleFor(x => x.CatalogName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty();

            When(x => x.Categories.Any(), () =>
            {
                RuleForEach(x => x.Categories).ChildRules(category =>
                {
                    category.RuleFor(x => x.DisplayName)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull()
                        .NotEmpty();

                    category.RuleFor(x => x.CategoryId)
                        .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull()
                        .NotEqual(Guid.Empty);
                });
            });
        }
    }
}
