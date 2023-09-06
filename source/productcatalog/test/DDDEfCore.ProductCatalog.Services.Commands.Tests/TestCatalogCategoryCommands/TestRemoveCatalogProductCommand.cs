using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCategoryCommands.RemoveCatalogProduct;
using FakeItEasy;
using FluentValidation.TestHelper;
using MockQueryable.FakeItEasy;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCategoryCommands;

public class TestRemoveCatalogProductCommand
{
    private readonly Catalog _catalog;
    private readonly Category _category;
    private readonly Product _product;
    private readonly CatalogCategory _catalogCategory;
    private readonly CatalogProduct _catalogProduct;

    private readonly IRepository<Catalog, CatalogId> _catalogRepository;

    public TestRemoveCatalogProductCommand()
    {
        this._catalogRepository = A.Fake<IRepository<Catalog, CatalogId>>();

        this._catalog = Catalog.Create("Catalog");
        this._category = Category.Create("Category");
        this._product = Product.Create("Product");

        this._catalogCategory = this._catalog.AddCategory(this._category.Id, "Catalog-Category");
        this._catalogProduct = this._catalogCategory.CreateCatalogProduct(this._product.Id, "Catalog-Product");

        A.CallTo(() => this._catalogRepository.AsQueryable()).Returns(new List<Catalog> { this._catalog }.BuildMock());
    }

    [Fact(DisplayName = "Remove CatalogProduct Successfully")]
    public async Task Remove_CatalogProduct_Successfully()
    {
        var command = new RemoveCatalogProductCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = this._catalogCategory.Id,
            CatalogProductId = this._catalogProduct.Id
        };

        var commandHandler = new CommandHandler(this._catalogRepository);

        var result = await commandHandler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.CatalogProductId.ShouldBe(this._catalogProduct.Id);
    }


    [Fact(DisplayName = "Validation: Empty Command Should Be Invalid")]
    public async Task Empty_Command_ShouldBe_Invalid()
    {
        var command = new RemoveCatalogProductCommand
        {
            CatalogId = CatalogId.Empty,
            CatalogCategoryId = CatalogCategoryId.Empty,
            CatalogProductId = CatalogProductId.Empty
        };

        var validator = new RemoveCatalogProductCommandValidator(this._catalogRepository);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
        result.ShouldHaveValidationErrorFor(x => x.CatalogProductId);
    }

    [Fact(DisplayName = "Validation: CatalogCategory Not Found Should Be Invalid")]
    public async Task CatalogCategory_NotFound_ShouldBe_Invalid()
    {
        var command = new RemoveCatalogProductCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = CatalogCategoryId.New,
            CatalogProductId = CatalogProductId.New
        };

        var validator = new RemoveCatalogProductCommandValidator(this._catalogRepository);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogProductId);
    }

    [Fact(DisplayName = "Validation: CatalogProduct Not Found Should Be Invalid")]
    public async Task CatalogProduct_NotFound_ShouldBe_Invalid()
    {
        var command = new RemoveCatalogProductCommand
        {
            CatalogId = this._catalog.Id,
            CatalogCategoryId = this._catalogCategory.Id,
            CatalogProductId = CatalogProductId.New
        };

        var validator = new RemoveCatalogProductCommandValidator(this._catalogRepository);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CatalogProductId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
    }
}
