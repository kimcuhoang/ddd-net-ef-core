using System;
using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty)
                .WithMessage($"{nameof(UpdateCategoryCommand.CategoryId)} is empty or invalid.");

            When(x => x.CategoryId.IsNotEmpty, () =>
            {
                RuleFor(x => x.CategoryId).Custom((categoryId, context) =>
                {
                    var repository = repositoryFactory.CreateRepository<Category>();
                    var category = repository.FindOneAsync(x => x.CategoryId == categoryId).GetAwaiter().GetResult();

                    if (category == null)
                    {
                        context.AddFailure(nameof(UpdateCategoryCommand.CategoryId), $"Could not found Category#{categoryId}");
                    }
                });
            });

            RuleFor(x => x.CategoryName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEmpty();
        }
    }
}
