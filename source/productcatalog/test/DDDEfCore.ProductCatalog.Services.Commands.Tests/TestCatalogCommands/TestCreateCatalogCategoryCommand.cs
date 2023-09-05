using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands;

/// <summary>
/// https://github.com/MichalJankowskii/Moq.EntityFrameworkCore
/// </summary>
public class TestCreateCatalogCategoryCommand
{
    private readonly Catalog _catalog;
    private readonly Category _category;

    private readonly Mock<IRepository<Catalog, CatalogId>> _mockCatalogRepository;
    private readonly Mock<IRepository<Category, CategoryId>> _mockCategoryRepository;
    private readonly IFixture _fixture;

    public TestCreateCatalogCategoryCommand()
    {
        this._mockCatalogRepository = new Mock<IRepository<Catalog, CatalogId>>();
        this._mockCategoryRepository = new Mock<IRepository<Category, CategoryId>>();
        this._fixture = new Fixture();

        this._catalog = Catalog.Create("Catalog");
        this._category = Category.Create("Category");

        this._mockCatalogRepository
            .Setup(_ => _.AsQueryable())
            .Returns(new List<Catalog> { this._catalog }.BuildMock());

        this._mockCategoryRepository
            .Setup(_ => _.AsQueryable())
            .Returns(new List<Category> { this._category }.BuildMock());
    }

    [Fact(DisplayName = "Create CatalogCategory Successfully")]
    public async Task Create_CatalogCategory_Successfully()
    {
        var command = new CreateCatalogCategoryCommand
        {
            CatalogId = this._catalog.Id,
            CategoryId = this._category.Id,
            DisplayName = this._fixture.Create<string>()
        };

        var commandHandler = new CommandHandler(this._mockCatalogRepository.Object);

        var result = await commandHandler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.CatalogCategoryId.ShouldNotBeNull().ShouldNotBe(CatalogCategoryId.Empty);
    }

    [Fact(DisplayName = "Create CatalogCategory As Child Successfully")]
    public async Task Create_CatalogCategory_As_Child_Successfully()
    {
        var catalogCategory = this._catalog.AddCategory(this._category.Id, this._category.DisplayName);
        var childCategory = Category.Create(this._fixture.Create<string>());

        var command = new CreateCatalogCategoryCommand
        {
            CatalogId = this._catalog.Id,
            CategoryId = childCategory.Id,
            DisplayName = childCategory.DisplayName,
            ParentCatalogCategoryId = catalogCategory.Id
        };

        var commandHandler = new CommandHandler(this._mockCatalogRepository.Object);

        var result = await commandHandler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.CatalogCategoryId.ShouldNotBeNull().ShouldNotBe(CatalogCategoryId.Empty);
    }

    [Fact(DisplayName = "Command With Empty Values Should Be Invalid")]
    public async Task Command_With_Empty_Values_ShouldBeInvalid()
    {
        var command = new CreateCatalogCategoryCommand
        {
            CatalogId = CatalogId.Empty,
            CategoryId = CategoryId.Empty,
            DisplayName = string.Empty
        };

        var validator = new CreateCatalogCategoryCommandValidator(this._mockCatalogRepository.Object,
                                                                this._mockCategoryRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact(DisplayName = "Command With Not Found Catalog Should Be Invalid")]
    public async Task Command_With_NotFound_Catalog_ShouldBeInvalid()
    {
        var command = new CreateCatalogCategoryCommand
        {
            CatalogId = CatalogId.New,
            CategoryId = this._category.Id,
            DisplayName = this._category.DisplayName
        };

        this._mockCategoryRepository
            .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(this._category);

        var validator = new CreateCatalogCategoryCommandValidator(this._mockCatalogRepository.Object,
                                                                this._mockCategoryRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact(DisplayName = "Command With Not Found Category Should Be Invalid")]
    public async Task Command_With_NotFound_Category_ShouldBeInvalid()
    {
        var command = new CreateCatalogCategoryCommand
        {
            CatalogId = this._catalog.Id,
            CategoryId = CategoryId.New,
            DisplayName = this._fixture.Create<string>()
        };

        this._mockCatalogRepository
            .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Catalog, bool>>>()))
            .ReturnsAsync(this._catalog);

        var validator = new CreateCatalogCategoryCommandValidator(this._mockCatalogRepository.Object,
                                                                this._mockCategoryRepository.Object);

        var result = await validator.TestValidateAsync(command);


        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact(DisplayName = "Command With Invalid ParentCatalogCategoryId Should Be Invalid")]
    public async Task Command_With_Invalid_ParentCatalogCategoryId_ShouldBeInvalid()
    {

        var command = new CreateCatalogCategoryCommand
        {
            CatalogId = this._catalog.Id,
            CategoryId = this._category.Id,
            DisplayName = this._fixture.Create<string>(),
            ParentCatalogCategoryId = CatalogCategoryId.New
        };

        this._mockCatalogRepository
            .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Catalog, bool>>>()))
            .ReturnsAsync(this._catalog);

        this._mockCategoryRepository
            .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(this._category);

        var validator = new CreateCatalogCategoryCommandValidator(this._mockCatalogRepository.Object,
                                                                this._mockCategoryRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        result.ShouldHaveValidationErrorFor(x => x.ParentCatalogCategoryId);
    }
}
