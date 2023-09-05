using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CategoryCommands.CreateCategory;
using FluentValidation.TestHelper;
using Moq;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCategoryCommands;

public class TestCreateCategoryCommand
{
    private readonly Mock<IRepository<Category, CategoryId>> _mockCategoryRepository;
    private readonly IFixture _fixture;

    public TestCreateCategoryCommand()
    {
        this._mockCategoryRepository = new Mock<IRepository<Category, CategoryId>>();
        this._fixture = new Fixture();
    }

    [Fact(DisplayName = "Create Category Successfully")]
    public async Task Create_Category_Successfully()
    {
        var command = this._fixture.Create<CreateCategoryCommand>();

        var handler = new CommandHandler(this._mockCategoryRepository.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.CategoryId.ShouldNotBeNull().ShouldNotBe(CategoryId.Empty);
    }

    

    [Fact(DisplayName = "CreateCategoryCommand Has Empty CategoryName Should Be Invalid")]
    public void CreateCategoryCommand_Has_Empty_CategoryName_ShouldBeInvalid()
    {
        var command = new CreateCategoryCommand { CategoryName = string.Empty };

        var validator = new CreateCategoryCommandValidator();
        var result = validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CategoryName);
    }
}
