﻿using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.RemoveCatalogCategory;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands;

public class TestRemoveCatalogCategoryCommand
{
    private readonly Mock<IRepository<Catalog, CatalogId>> _mockCatalogRepository;
    private readonly IFixture _fixture;

    public TestRemoveCatalogCategoryCommand()
    {
        this._mockCatalogRepository = new Mock<IRepository<Catalog, CatalogId>>();
        this._fixture = new Fixture();
    }

    [Fact(DisplayName = "Remove CatalogCategory Successfully")]
    public async Task Remove_CatalogCategory_Successfully()
    {
        var catalog = Catalog.Create(this._fixture.Create<string>());
        var catalogs = new List<Catalog> { catalog };

        var categoryId = CategoryId.New;
        var catalogCategory = catalog.AddCategory(categoryId, this._fixture.Create<string>());

        var command = new RemoveCatalogCategoryCommand
        {
            CatalogId = catalog.Id,
            CatalogCategoryId = catalogCategory.Id
        };

        this._mockCatalogRepository
            .Setup(_ => _.AsQueryable())
            .Returns(catalogs.BuildMock());

        var handler = new CommandHandler(this._mockCatalogRepository.Object);

        var result = await handler.Handle(command, CancellationToken.None);

    }

    [Fact(DisplayName = "Validate: Guid.Empty Ids Should Be Invalid")]
    public async Task GuidEmpty_For_Ids_ShouldBeInvalid()
    {
        var command = new RemoveCatalogCategoryCommand
        {
            CatalogId = CatalogId.Empty,
            CatalogCategoryId = CatalogCategoryId.Empty
        };

        var validator = new RemoveCatalogCategoryCommandValidator(this._mockCatalogRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
    }

    [Fact(DisplayName = "Validate: Catalog Not Found Should Be Invalid")]
    public async Task Catalog_NotFound_ShouldBeInvalid()
    {
        var command = new RemoveCatalogCategoryCommand
        {
            CatalogId = CatalogId.Empty,
            CatalogCategoryId = CatalogCategoryId.New
        };

        var validator = new RemoveCatalogCategoryCommandValidator(this._mockCatalogRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
    }

    [Fact(DisplayName = "Validate: CatalogCategory Not Found Should Fail Validation")]
    public async Task CatalogCategory_NotFound_ShouldBeInvalid()
    {
        var catalog = Catalog.Create(this._fixture.Create<string>());
        var catalogs = new List<Catalog> { catalog };
        
        this._mockCatalogRepository
            .Setup(_ => _.AsQueryable())
            .Returns(catalogs.BuildMock());

        var command = new RemoveCatalogCategoryCommand
        {
            CatalogId = catalog.Id,
            CatalogCategoryId = CatalogCategoryId.Empty
        };

        var validator = new RemoveCatalogCategoryCommandValidator(this._mockCatalogRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
    }
}
