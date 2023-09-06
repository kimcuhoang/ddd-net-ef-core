using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory;

public class CommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResult>
{
    private readonly IRepository<Category, CategoryId> _repository;

    public CommandHandler(IRepository<Category, CategoryId> repository)
    {
        this._repository = repository;
    }

    public async Task<UpdateCategoryResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await this._repository.FindOneAsync(x => x.Id == request.CategoryId);

        category.ChangeDisplayName(request.CategoryName);

        return new UpdateCategoryResult
        {
            CategoryId = request.CategoryId
        };
    }
}
