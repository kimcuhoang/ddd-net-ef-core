using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Categories;
using FluentValidation;

namespace DDD.ProductCatalog.Application.Commands.CategoryCommands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator(IRepository<Category, CategoryId> categoryRepository)
    {
        RuleFor(x => x.CategoryId).NotNull().NotEqual(CategoryId.Empty);

        When(x => x.CategoryId is not null && x.CategoryId != CategoryId.Empty, () =>
        {
            RuleFor(x => x.CategoryId).CustomAsync(async (categoryId, context, token) =>
            {
                var category = await categoryRepository.FindOneAsync(x => x.Id == categoryId);

                if (category is null)
                {
                    context.AddFailure(nameof(UpdateCategoryCommand.CategoryId), $"Could not found Category#{categoryId}");
                }
            });
        });

        RuleFor(x => x.CategoryName)
            .NotNull()
            .NotEmpty();
    }
}
