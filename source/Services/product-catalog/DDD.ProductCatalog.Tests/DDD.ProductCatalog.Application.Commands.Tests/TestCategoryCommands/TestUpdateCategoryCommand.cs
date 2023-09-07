using DNK.DDD.Core;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Application.Commands.CategoryCommands.UpdateCategory;
using FakeItEasy;
using FluentValidation.TestHelper;

namespace DDD.ProductCatalog.Application.Commands.Tests.TestCategoryCommands;

public class TestUpdateCategoryCommand
{
    private readonly IRepository<Category, CategoryId> _categoryRepository;
    private readonly IFixture _fixture;
    private readonly Category _category;

    public TestUpdateCategoryCommand()
    {
        this._categoryRepository = A.Fake<IRepository<Category, CategoryId>>();
        this._fixture = new Fixture();
        this._category = Category.Create("Category");
    }

    [Fact(DisplayName = "Update Category Successfully")]
    public async Task Update_Category_Successfully()
    {
        A.CallTo(() => this._categoryRepository.FindOneAsync(default!))
            .WithAnyArguments()
            .Returns(Task.FromResult((Category?)this._category));

        var command = new UpdateCategoryCommand
        {
            CategoryId = this._category.Id,
            CategoryName = this._fixture.Create<string>()
        };

        var handler = new CommandHandler(this._categoryRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.CategoryId.ShouldNotBeNull().ShouldBe(command.CategoryId);
    }

    [Fact(DisplayName = "Update Not Found Category Should not pass validator")]
    public async Task Update_NotFound_Category_ShouldNotPassValidator()
    {
        var command = new UpdateCategoryCommand
        {
            CategoryId = CategoryId.New,
            CategoryName = this._fixture.Create<string>()
        };

        var validator = new UpdateCategoryCommandValidator(this._categoryRepository);
        var validationResult = await validator.TestValidateAsync(command);

        validationResult.ShouldHaveValidationErrorFor(x => x.CategoryId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.CategoryName);
    }
}
