using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Categories;
using FluentValidation;

namespace DDD.ProductCatalog.Application.Commands.CatalogCommands.CreateCatalog;

public class CreateCatalogCommandValidator : AbstractValidator<CreateCatalogCommand>
{
    public CreateCatalogCommandValidator(IRepository<Category, CategoryId> categoryRepository)
    {
        RuleFor(x => x.CatalogName)
            .NotNull()
            .NotEmpty();

        When(x => x.Categories.Any(), () =>
        {
            RuleForEach(x => x.Categories).ChildRules(category =>
            {
                category.RuleFor(x => x.DisplayName)
                    .NotNull()
                    .NotEmpty();

                category.RuleFor(x => x.CategoryId)
                    .NotNull().NotEqual(CategoryId.Empty)
                    .MustAsync((categoryId, token) => CategoryMustExist(categoryRepository, categoryId, token))
                    .WithMessage(x => $"Category#{x.CategoryId} could not be found.");
            });
        });
    }

    private async Task<bool> CategoryMustExist(IRepository<Category, CategoryId> categoryRepository, CategoryId categoryId, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.FindOneAsync(x => x.Id == categoryId);
        return category != null;
    }
}
