using AutoFixture;
using AutoFixture.Xunit2;
using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.CreateCatalog;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using Shouldly;
using System.Linq.Expressions;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands;

public class TestCreateCatalogCommand : UnitTestBase
{
    private readonly CreateCatalogCommandValidator _validator;
    private readonly Mock<IRepository<Catalog, CatalogId>> _mockCatalogRepository;
    private readonly Mock<IRepository<Category, CategoryId>> _mockCategoryRepository;

    public TestCreateCatalogCommand() : base()
    {
        this._mockCategoryRepository = new Mock<IRepository<Category, CategoryId>>();
        this._mockCatalogRepository = new Mock<IRepository<Catalog, CatalogId>>();

        this.MockRepositoryFactory
            .Setup(_ => _.CreateRepository<Catalog, CatalogId>())
            .Returns(this._mockCatalogRepository.Object);

        this.MockRepositoryFactory
            .Setup(_ => _.CreateRepository<Category, CategoryId>())
            .Returns(this._mockCategoryRepository.Object);

        this._validator = new CreateCatalogCommandValidator(this.MockRepositoryFactory.Object);
    }

    [Theory(DisplayName = "Create Catalog Without CatalogCategory Successfully")]
    [AutoData]
    public async Task Create_Catalog_Without_CatalogCategory_Successfully(string catalogName)
    {
        var command = new CreateCatalogCommand
        {
            CatalogName = catalogName
        };

        var handler = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

        await handler.Handle(command, this.CancellationToken);

        this._mockCatalogRepository.Verify(x => x.AddAsync(It.IsAny<Catalog>()), Times.Once);
    }

    [Theory(DisplayName = "Create Catalog With CatalogCategories Successfully")]
    [AutoData]
    public async Task Create_Catalog_With_CatalogCategories_Successfully(string catalogName)
    {
        var categories = Enumerable.Range(0, 4)
            .Select(idx => Category.Create(this.Fixture.Create<string>()))
            .ToList();

        var command = new CreateCatalogCommand
        {
            CatalogName = catalogName
        };
        foreach (var category in categories)
        {
            command.AddCategory(category.Id, this.Fixture.Create<string>());
        }

        this._mockCategoryRepository
                .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(categories.First());

        var handler = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

        await handler.Handle(command, this.CancellationToken);

        this._mockCatalogRepository.Verify(x => x.AddAsync(It.IsAny<Catalog>()), Times.Once);
    }

    [Fact(DisplayName = "Create Catalog With Invalid Command Should Throw Exception")]
    public async Task Create_Catalog_With_Invalid_Command_ShouldThrowException()
    {
        var command = new CreateCatalogCommand();
        command.AddCategory(CategoryId.Empty, string.Empty);

        var handler = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

        await Should.ThrowAsync<ValidationException>(async () =>
            await handler.Handle(command, this.CancellationToken));
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
            CatalogName = this.Fixture.Create<string>()
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
