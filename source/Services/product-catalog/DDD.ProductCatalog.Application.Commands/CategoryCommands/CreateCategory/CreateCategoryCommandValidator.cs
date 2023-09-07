using FluentValidation;

namespace DDD.ProductCatalog.Application.Commands.CategoryCommands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(command => command.CategoryName)
            .NotNull()
            .NotEmpty();
    }
}
