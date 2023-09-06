using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalogCategory;
using FakeItEasy;
using FluentValidation.TestHelper;
using MockQueryable.FakeItEasy;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands;

/// <summary>
/// https://github.com/MichalJankowskii/Moq.EntityFrameworkCore
/// </summary>
public class TestCreateCatalogCategoryCommand
{
    private readonly Catalog _catalog;
    private readonly Category _category;

    private readonly IRepository<Catalog, CatalogId> _catalogRepository;
    private readonly IRepository<Category, CategoryId> _categoryRepository;
    private readonly IFixture _fixture;

    public TestCreateCatalogCategoryCommand()
    {
        this._catalogRepository = A.Fake<IRepository<Catalog, CatalogId>>();
        this._categoryRepository = A.Fake<IRepository<Category, CategoryId>>();
        this._fixture = new Fixture();

        this._catalog = Catalog.Create("Catalog");
        this._category = Category.Create("Category");

        A.CallTo(() => this._catalogRepository.AsQueryable()).Returns(new List<Catalog> { this._catalog }.BuildMock());
        A.CallTo(() => this._categoryRepository.AsQueryable()).Returns(new List<Category> { this._category }.BuildMock());
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

        var commandHandler = new CommandHandler(this._catalogRepository);

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

        var commandHandler = new CommandHandler(this._catalogRepository);

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

        var validator = new CreateCatalogCategoryCommandValidator(this._catalogRepository,
                                                                this._categoryRepository);

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

        A.CallTo(() => this._categoryRepository.FindOneAsync(default!))
            .WithAnyArguments()
            .Returns(Task.FromResult((Category?)this._category));

        var validator = new CreateCatalogCategoryCommandValidator(this._catalogRepository,
                                                                this._categoryRepository);

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

        A.CallTo(() => this._catalogRepository.FindOneAsync(default!))
            .WithAnyArguments()
            .Returns(Task.FromResult((Catalog?)this._catalog));

        var validator = new CreateCatalogCategoryCommandValidator(this._catalogRepository,
                                                                this._categoryRepository);

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

        A.CallTo(() => this._catalogRepository.FindOneAsync(default!))
            .WithAnyArguments().Returns(Task.FromResult((Catalog?)this._catalog));

        A.CallTo(() => this._categoryRepository.FindOneAsync(default!))
            .WithAnyArguments().Returns(Task.FromResult((Category?)this._category));

        var validator = new CreateCatalogCategoryCommandValidator(this._catalogRepository,
                                                                this._categoryRepository);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
        result.ShouldHaveValidationErrorFor(x => x.ParentCatalogCategoryId);
    }
}
