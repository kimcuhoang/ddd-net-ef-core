using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands.ProductCommands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.ProductName)
            .NotNull()
            .NotEmpty();
    }
}
