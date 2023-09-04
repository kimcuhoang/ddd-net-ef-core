using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using FluentValidation;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory;

public class CommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IRepository<Category, CategoryId> _repository;

    private readonly IValidator<UpdateCategoryCommand> _validator;

    public CommandHandler(IRepository<Category, CategoryId> repository, IValidator<UpdateCategoryCommand> validator)
    {
        this._repository = repository;
        this._validator = validator;
    }

    #region Overrides of IRequestHandler<UpdateCategoryCommand>

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        await this._validator.ValidateAndThrowAsync(request, cancellationToken);

        var category = await this._repository.FindOneAsync(x => x.Id == request.CategoryId);
        
        category.ChangeDisplayName(request.CategoryName);

        //await this._repository.UpdateAsync(category);
    }

    #endregion
}
