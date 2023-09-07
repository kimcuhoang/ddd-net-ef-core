using FluentValidation;

namespace DDD.ProductCatalog.Application.Commands.ProductCommands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.ProductName)
            .NotNull()
            .NotEmpty();
    }
}
