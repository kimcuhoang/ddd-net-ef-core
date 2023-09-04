using AutoFixture;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCategoryCommands;

public class TestUpdateCatalogCategoryCommand : UnitTestBase<Catalog, CatalogId>, IAsyncLifetime
{
    private Mock<DbContext> _mockDbContext;
    private UpdateCatalogCategoryCommandValidator _validator;
    private IRequestHandler<UpdateCatalogCategoryCommand> _requestHandler;

    private Catalog _catalog;
    private Category _category;
    private CatalogCategory _catalogCategory;

    #region Implementation of IAsyncLifetime

    public async Task InitializeAsync()
    {
        await Task.Yield();

        this._catalog = Catalog.Create(this.Fixture.Create<string>());
        this._category = Category.Create(this.Fixture.Create<string>());
        this._catalogCategory = this._catalog.AddCategory(this._category.Id, this._category.DisplayName);

        this._mockDbContext = new Mock<DbContext>();
        this._mockDbContext
            .Setup(_ => _.Set<Catalog>())
            .ReturnsDbSet(new List<Catalog> { this._catalog });

        var catalogRepository = new DefaultRepositoryAsync<Catalog, CatalogId>(this._mockDbContext.Object);

        this._validator = new UpdateCatalogCategoryCommandValidator(catalogRepository);
        this._requestHandler = new CommandHandler(catalogRepository, this._validator);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    #endregion

    [Fact(DisplayName = "Update CatalogCategory Successfully")]
    public async Task Update_CatalogCategory_Successfully()
    {
        var command = new UpdateCatalogCategoryCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = this._catalogCategory.Id,
            DisplayName = this.Fixture.Create<string>()
        };

        await this._requestHandler.Handle(command, this.CancellationToken);

        this._mockDbContext.Verify(x => x.Update(It.IsAny<Catalog>()), Times.Once);
    }

    [Fact(DisplayName = "Validate Fail Should Throw Exception")]
    public async Task Validation_Fail_ShouldThrowException()
    {
        var command = new UpdateCatalogCategoryCommand
        {
            CatalogId = CatalogId.Empty,
            CatalogCategoryId = CatalogCategoryId.Empty,
            DisplayName = this.Fixture.Create<string>()
        };

        await Should.ThrowAsync<ValidationException>(async () =>
            await this._requestHandler.Handle(command, this.CancellationToken));
    }

    [Fact(DisplayName = "Not Found Catalog Should Be Invalid")]
    public async Task Not_Found_Catalog_ShouldBeInvalid()
    {
        var command = new UpdateCatalogCategoryCommand
        {
            CatalogId = CatalogId.New,
            CatalogCategoryId = CatalogCategoryId.New,
            DisplayName = this.Fixture.Create<string>()
        };

        var result = await this._validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact(DisplayName = "Not Found CatalogCategory Should Be Invalid")]
    public async Task Not_Found_CatalogCategory_ShouldBeInvalid()
    {
        var command = new UpdateCatalogCategoryCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = CatalogCategoryId.New,
            DisplayName = this.Fixture.Create<string>()
        };

        var result = await this._validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact(DisplayName = "Empty DisplayName Should Be Invalid")]
    public async Task Empty_DisplayName_ShouldBeInvalid()
    {
        var command = new UpdateCatalogCategoryCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = this._catalogCategory.Id,
            DisplayName = string.Empty
        };

        var result = await this._validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
    }
}
