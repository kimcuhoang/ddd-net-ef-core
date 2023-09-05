﻿using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.CreateCatalogProduct;
using FluentValidation.TestHelper;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCategoryCommands;

public class TestCreateCatalogProductCommand
{
    private Catalog _catalog;
    private Category _category;
    private Product _product;
    private CatalogCategory _catalogCategory;

    private readonly Mock<IRepository<Catalog, CatalogId>> _mockCatalogRepository;
    private readonly Mock<IRepository<Product, ProductId>> _mockProductRepository;

    public TestCreateCatalogProductCommand()
    {
        this._mockCatalogRepository = new Mock<IRepository<Catalog, CatalogId>>();
        this._mockProductRepository = new Mock<IRepository<Product, ProductId>>();

        this._catalog = Catalog.Create("Catalog");
        this._category = Category.Create("Category");
        this._product = Product.Create("Product");

        this._catalogCategory = this._catalog.AddCategory(this._category.Id, "Catalog-Category");

        this._mockCatalogRepository
            .Setup(_ => _.AsQueryable())
            .Returns(new List<Catalog> { this._catalog }.BuildMock());

        this._mockProductRepository
            .Setup(_ => _.AsQueryable())
            .Returns(new List<Product> { this._product }.BuildMock());
    }



    [Fact(DisplayName = "Create CatalogProduct Successfully")]
    public async Task Create_CatalogProduct_Successfully()
    {
        var command = new CreateCatalogProductCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = this._catalogCategory.Id,
            ProductId = this._product.Id,
            DisplayName = this._product.Name
        };

        var commandHandler = new CommandHandler(this._mockCatalogRepository.Object);

        var result = await commandHandler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.CatalogProductId.ShouldNotBeNull().ShouldNotBe(CatalogProductId.Empty);
    }


    [Fact(DisplayName = "Empty Command Should Be Invalid")]
    public async Task Empty_Command_ShouldBe_Invalid()
    {
        var command = new CreateCatalogProductCommand
        {
            CatalogId = CatalogId.Empty,
            CatalogCategoryId = CatalogCategoryId.Empty,
            ProductId = ProductId.Empty,
            DisplayName = string.Empty
        };

        var validator = new CreateCatalogProductCommandValidator(this._mockCatalogRepository.Object, this._mockProductRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact(DisplayName = "Catalog Not Found Should Be Invalid")]
    public async Task Catalog_NotFound_ShouldBe_Invalid()
    {
        var command = new CreateCatalogProductCommand
        {
            CatalogId = CatalogId.New,
            CatalogCategoryId = CatalogCategoryId.New,
            ProductId = this._product.Id,
            DisplayName = this._product.Name
        };

        this._mockProductRepository
            .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(this._product);

        var validator = new CreateCatalogProductCommandValidator(this._mockCatalogRepository.Object, this._mockProductRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact(DisplayName = "CatalogCategory Not Found Should Be Invalid")]
    public async Task CatalogCategory_NotFound_ShouldBe_Invalid()
    {
        var command = new CreateCatalogProductCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = CatalogCategoryId.New,
            ProductId = this._product.Id,
            DisplayName = this._product.Name
        };

        this._mockProductRepository
            .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(this._product);

        var validator = new CreateCatalogProductCommandValidator(this._mockCatalogRepository.Object, this._mockProductRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact(DisplayName = "Product Not Found Should Be Invalid")]
    public async Task Product_NotFound_ShouldBe_Invalid()
    {
        var command = new CreateCatalogProductCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = this._catalogCategory.Id,
            ProductId = ProductId.New,
            DisplayName = this._product.Name
        };

        var validator = new CreateCatalogProductCommandValidator(this._mockCatalogRepository.Object, this._mockProductRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ProductId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact(DisplayName = "Duplicate Product Should Be Invalid")]
    public async Task Duplicate_Product_ShouldBe_Invalid()
    {
        var catalogProduct = this._catalogCategory.CreateCatalogProduct(this._product.Id, this._product.Name);

        var command = new CreateCatalogProductCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = this._catalogCategory.Id,
            ProductId = this._product.Id,
            DisplayName = this._product.Name
        };

        this._mockProductRepository
            .Setup(_ => _.FindOneAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(this._product);

        var validator = new CreateCatalogProductCommandValidator(this._mockCatalogRepository.Object, this._mockProductRepository.Object);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.ProductId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }
}
