﻿using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.UpdateCatalogCategory;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCategoryCommands;

public class TestUpdateCatalogCategoryCommand
{
    private Catalog _catalog;
    private Category _category;
    private CatalogCategory _catalogCategory;

    private readonly Mock<IRepository<Catalog, CatalogId>> _mockCatalogRepository;

    public TestUpdateCatalogCategoryCommand()
    {
        this._catalog = Catalog.Create("Catalog");
        this._category = Category.Create("Category");
        this._catalogCategory = this._catalog.AddCategory(this._category.Id, this._category.DisplayName);

        this._mockCatalogRepository = new Mock<IRepository<Catalog, CatalogId>>();
        this._mockCatalogRepository
            .Setup(_ => _.AsQueryable())
            .Returns(new List<Catalog> { this._catalog }.BuildMock());
    }

    

    [Fact(DisplayName = "Update CatalogCategory Successfully")]
    public async Task Update_CatalogCategory_Successfully()
    {
        var command = new UpdateCatalogCategoryCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = this._catalogCategory.Id,
            DisplayName = "Catalog-Category"
        };

        var commandHandler = new CommandHandler(this._mockCatalogRepository.Object);

        var result = await commandHandler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.CatalogCategoryId.ShouldBe(command.CatalogCategoryId);
    }


    [Fact(DisplayName = "Not Found Catalog Should Be Invalid")]
    public async Task Not_Found_Catalog_ShouldBeInvalid()
    {
        var command = new UpdateCatalogCategoryCommand
        {
            CatalogId = CatalogId.New,
            CatalogCategoryId = CatalogCategoryId.New,
            DisplayName = "Catalog-Category"
        };

        var validator = new UpdateCatalogCategoryCommandValidator(this._mockCatalogRepository.Object);
        var result = await validator.TestValidateAsync(command);

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
            DisplayName = "Catalog-Category"
        };

        var validator = new UpdateCatalogCategoryCommandValidator(this._mockCatalogRepository.Object);
        var result = await validator.TestValidateAsync(command);

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

        var validator = new UpdateCatalogCategoryCommandValidator(this._mockCatalogRepository.Object);
        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
    }
}
