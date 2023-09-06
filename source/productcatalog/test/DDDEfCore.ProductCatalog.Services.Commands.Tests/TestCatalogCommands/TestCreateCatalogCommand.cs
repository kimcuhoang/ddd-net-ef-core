using AutoFixture.Xunit2;
using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;
using FakeItEasy;
using FluentValidation.TestHelper;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands;

public class TestCreateCatalogCommand
{
    private readonly CreateCatalogCommandValidator _validator;
    private readonly IRepository<Catalog, CatalogId> _catalogRepository;
    private readonly IRepository<Category, CategoryId> _categoryRepository;
    private readonly IFixture _fixture;

    public TestCreateCatalogCommand() : base()
    {
        this._categoryRepository = A.Fake<IRepository<Category, CategoryId>>();
        this._catalogRepository = A.Fake<IRepository<Catalog, CatalogId>>();
        this._validator = new CreateCatalogCommandValidator(this._categoryRepository);
        this._fixture = new Fixture();
    }

    [Theory(DisplayName = "Create Catalog Without CatalogCategory Successfully")]
    [AutoData]
    public async Task Create_Catalog_Without_CatalogCategory_Successfully(string catalogName)
    {
        var command = new CreateCatalogCommand
        {
            CatalogName = catalogName
        };

        var handler = new CommandHandler(this._catalogRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        A.CallTo(() => this._catalogRepository.Add(default!))
            .WhenArgumentsMatch(args => args.First() is Catalog)
            .MustHaveHappenedOnceExactly();

        result.ShouldNotBeNull();
        result.CatalogId.ShouldNotBeNull();
        result.CatalogId.ShouldNotBe(CatalogId.Empty);
    }

    [Theory(DisplayName = "Create Catalog With CatalogCategories Successfully")]
    [AutoData]
    public async Task Create_Catalog_With_CatalogCategories_Successfully(string catalogName)
    {
        var categories = Enumerable.Range(0, 4)
            .Select(idx => Category.Create(this._fixture.Create<string>()))
            .ToList();

        var command = new CreateCatalogCommand
        {
            CatalogName = catalogName
        };
        foreach (var category in categories)
        {
            command.AddCategory(category.Id, this._fixture.Create<string>());
        }

        A.CallTo(() => this._categoryRepository.FindOneAsync(default!))
            .WithAnyArguments()
            .Returns(Task.FromResult((Category?)categories.First()));

        var handler = new CommandHandler(this._catalogRepository);

        await handler.Handle(command, CancellationToken.None);

        A.CallTo(() => this._catalogRepository.Add(default!))
            .WhenArgumentsMatch(args => args.First() is Catalog)
            .MustHaveHappenedOnceExactly();
    }

    [Fact(DisplayName = "Create Catalog With Invalid Command Should Throw Exception")]
    public async Task Create_Catalog_With_Invalid_Command_ShouldThrowException()
    {
        var command = new CreateCatalogCommand();
        command.AddCategory(CategoryId.Empty, string.Empty);

        var result = await this._validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogName);

        for (var i = 0; i < command.Categories.Count; i++)
        {
            result.ShouldHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}[{i}].{nameof(CreateCatalogCommand.CategoryInCatalog.DisplayName)}");
            result.ShouldHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}[{i}].{nameof(CreateCatalogCommand.CategoryInCatalog.CategoryId)}");
        }
    }

    [Fact(DisplayName = "CreateCatalogCommand With Empty CatalogName Should Be Invalid")]
    public async Task CreateCatalogCommand_With_Empty_CatalogName_ShouldBeInvalid()
    {
        var command = new CreateCatalogCommand();

        var result = await this._validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogName);

        result.ShouldNotHaveValidationErrorFor(
            $"{nameof(CreateCatalogCommand.Categories)}.{nameof(CreateCatalogCommand.CategoryInCatalog.DisplayName)}");
        result.ShouldNotHaveValidationErrorFor(
            $"{nameof(CreateCatalogCommand.Categories)}.{nameof(CreateCatalogCommand.CategoryInCatalog.CategoryId)}");
    }

    [Fact(DisplayName = "CreateCatalogCommand has Categories Without Id and DisplayName Should Be Invalid")]
    public async Task CreateCatalogCommand_Has_Categories_With_Empty_Id_And_DisplayName_ShouldBeInvalid()
    {
        var command = new CreateCatalogCommand
        {
            CatalogName = this._fixture.Create<string>()
        };
        command.AddCategory(CategoryId.Empty, string.Empty);

        var result = await this._validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CatalogName);

        for (var i = 0; i < command.Categories.Count; i++)
        {
            result.ShouldHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}[{i}].{nameof(CreateCatalogCommand.CategoryInCatalog.DisplayName)}");
            result.ShouldHaveValidationErrorFor(
                $"{nameof(CreateCatalogCommand.Categories)}[{i}].{nameof(CreateCatalogCommand.CategoryInCatalog.CategoryId)}");
        }
    }
}
