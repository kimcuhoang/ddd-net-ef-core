using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory;

public class CommandHandler : IRequestHandler<CreateCategoryCommand>
{
    private readonly IRepository<Category, CategoryId> _repository;

    private readonly IValidator<CreateCategoryCommand> _validator;

    public CommandHandler(IRepository<Category, CategoryId> repository, IValidator<CreateCategoryCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }



    #region Overrides of IRequestHandler<CreateCategoryCommand>

    public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);

        var category = Category.Create(request.CategoryName);

        await Task.Yield();

        this._repository.Add(category);
    }

    #endregion
}
