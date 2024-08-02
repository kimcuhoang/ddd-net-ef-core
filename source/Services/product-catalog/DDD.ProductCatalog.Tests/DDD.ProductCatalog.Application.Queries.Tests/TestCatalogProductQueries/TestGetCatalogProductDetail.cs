using DDD.ProductCatalog.Application.Queries.CatalogProductQueries.GetCatalogProductDetail;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestCatalogProductQueries;

public class TestGetCatalogProductDetail(TestQueriesCollectionFixture testFixture, ITestOutputHelper output) : TestQueriesBase(testFixture, output)
{
    private Product Product = default!;
    private Category Category = default!;
    private Catalog Catalog = default!;
    private CatalogCategory CatalogCategory = default!;
    private CatalogProduct CatalogProduct = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Category = Category.Create(this._fixture.Create<string>());
        this.Product = Product.Create(this._fixture.Create<string>());
        this.Catalog = Catalog.Create(this._fixture.Create<string>());

        this.CatalogCategory = this.Catalog.AddCategory(this.Category.Id, this.Category.DisplayName);
        this.CatalogProduct = this.CatalogCategory.CreateCatalogProduct(this.Product.Id, this.Product.Name);

        await this.ExecuteTransactionDbContext(async dbContext =>
        {
            dbContext.AddRange(this.Category, this.Product, this.Catalog);
            await dbContext.SaveChangesAsync();
        });
    }

    [Fact(DisplayName = "GetCatalogProductDetail Correctly")]
    public async Task GetCatalogProductDetail_Correctly()
    {
        var catalogProduct = this.CatalogProduct;
        var catalogProductId = catalogProduct.Id;
        var catalogCategory = this.CatalogCategory;
        var catalogCategoryId = catalogCategory.Id;
        var catalog = this.Catalog;
        var catalogId = catalog.Id;

        var request = new GetCatalogProductDetailRequest
        {
            CatalogProductId = catalogProductId
        };
        await this.ExecuteTestRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();

            result.CatalogProduct.ShouldNotBeNull();
            result.CatalogProduct.CatalogProductId.ShouldBe(catalogProductId);
            result.CatalogProduct.DisplayName.ShouldBe(catalogProduct.DisplayName);

            result.CatalogCategory.ShouldNotBeNull();
            result.CatalogCategory.CatalogCategoryId.ShouldBe(catalogCategoryId);
            result.CatalogCategory.DisplayName.ShouldBe(catalogCategory.DisplayName);

            result.Catalog.ShouldNotBeNull();
            result.Catalog.CatalogId.ShouldBe(catalogId);
            result.Catalog.CatalogName.ShouldBe(catalog.DisplayName);
        });
    }

    [Fact(DisplayName = "GetCatalogProductDetail Not Found CatalogProduct Still Work Correctly")]
    public async Task GetCatalogProductDetail_NotFound_CatalogProduct_Still_Work_Correctly()
    {
        var request = new GetCatalogProductDetailRequest
        {
            CatalogProductId = CatalogProductId.New
        };
        await this.ExecuteTestRequestHandler<GetCatalogProductDetailRequest, GetCatalogProductDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.IsNull.ShouldBe(true);
            result.CatalogCategory.ShouldNotBeNull();
            result.CatalogCategory.CatalogCategoryId.ShouldBeNull();
            string.IsNullOrWhiteSpace(result.CatalogCategory.DisplayName).ShouldBeTrue();

            result.Catalog.ShouldNotBeNull();
            result.Catalog.CatalogId.ShouldBeNull();
            string.IsNullOrWhiteSpace(result.Catalog.CatalogName).ShouldBeTrue();
        });
    }

    [Fact(DisplayName = "Should Validate Request Correctly")]
    public async Task Should_Validate_Request_Correctly()
    {
        var request = new GetCatalogProductDetailRequest
        {
            CatalogProductId = CatalogProductId.Empty
        };

        await this.ExecuteValidationTest(request,
            result => { result.ShouldHaveValidationErrorFor(x => x.CatalogProductId); });
    }
}
