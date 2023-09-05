using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.UpdateCategory;
using FluentValidation.TestHelper;
using Moq;
using System.Linq.Expressions;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCategoryCommands;

public class TestUpdateCategoryCommand
{
    private readonly Mock<IRepository<Category, CategoryId>> _mockCategoryRepository;
    private readonly IFixture _fixture;
    private readonly Category _category;

    public TestUpdateCategoryCommand()
    {
        this._mockCategoryRepository = new Mock<IRepository<Category, CategoryId>>();
        this._fixture = new Fixture();
        this._category = Category.Create("Category");
    }

    [Fact(DisplayName = "Update Category Successfully")]
    public async Task Update_Category_Successfully()
    {
        this._mockCategoryRepository
            .Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(this._category);

        var command = new UpdateCategoryCommand
        {
            CategoryId = this._category.Id,
            CategoryName = this._fixture.Create<string>()
        };

        var handler = new CommandHandler(this._mockCategoryRepository.Object);

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

        var validator = new UpdateCategoryCommandValidator(this._mockCategoryRepository.Object);
        var validationResult = await validator.TestValidateAsync(command);

        validationResult.ShouldHaveValidationErrorFor(x => x.CategoryId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.CategoryName);
    }
}
