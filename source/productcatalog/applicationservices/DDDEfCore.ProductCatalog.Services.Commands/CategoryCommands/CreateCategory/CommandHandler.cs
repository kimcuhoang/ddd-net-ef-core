using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using MediatR;

namespace DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory;

public class CommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResult>
{
    private readonly IRepository<Category, CategoryId> _repository;


    public CommandHandler(IRepository<Category, CategoryId> repository)
    {
        this._repository = repository;
    }
    public async Task<CreateCategoryResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.CategoryName);

        await Task.Yield();

        this._repository.Add(category);

        return new CreateCategoryResult
        {
            CategoryId = category.Id
        };
    }
}
