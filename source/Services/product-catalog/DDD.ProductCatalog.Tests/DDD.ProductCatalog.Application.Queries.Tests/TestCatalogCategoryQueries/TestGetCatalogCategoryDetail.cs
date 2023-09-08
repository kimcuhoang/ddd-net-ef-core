using DDD.ProductCatalog.Application.Queries.CatalogCategoryQueries.GetCatalogCategoryDetail;
using DDD.ProductCatalog.Core.Catalogs;
using DDD.ProductCatalog.Core.Categories;
using DDD.ProductCatalog.Core.Products;

namespace DDD.ProductCatalog.Application.Queries.Tests.TestCatalogCategoryQueries;

public class TestGetCatalogCategoryDetail : TestQueriesBase
{
    public TestGetCatalogCategoryDetail(TestQueriesFixture testFixture, ITestOutputHelper output) : base(testFixture, output)
    {
    }

    private Catalog Catalog = default!;
    private Category Category = default!;
    private Product Product = default!;
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
            dbContext.AddRange(this.Product, this.Category, this.Catalog);

            await dbContext.SaveChangesAsync();
        });
    }

    [Theory(DisplayName = "Should GetCatalogCategoryDetail With Paging CatalogProduct Correctly")]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(1, int.MaxValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public async Task Should_GetCatalogCategoryDetail_With_Paging_CatalogProduct_Correctly(int pageIndex, int pageSize)
    {
        var catalogCategory = this.CatalogCategory;
        var catalogCategoryId = catalogCategory.Id;
        var request = new GetCatalogCategoryDetailRequest
        {
            CatalogCategoryId = catalogCategoryId,
            CatalogProductCriteria = new GetCatalogCategoryDetailRequest.CatalogProductSearchRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }
        };

        await this.ExecuteTestRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.CatalogCategoryDetail.ShouldNotBeNull();
            result.CatalogCategoryDetail.CatalogId.ShouldBe(this.Catalog.Id);
            result.CatalogCategoryDetail.CatalogCategoryId.ShouldBe(catalogCategoryId);
            result.CatalogCategoryDetail.CatalogCategoryName.ShouldBe(this.CatalogCategory.DisplayName);
            result.CatalogCategoryDetail.CatalogName.ShouldBe(this.Catalog.DisplayName);

            result.TotalOfCatalogProducts.ShouldBe(catalogCategory.Products.Count());

            result.CatalogProducts.ToList().ForEach(c =>
            {
                var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x.Id == c.CatalogProductId);

                catalogProduct.ShouldNotBeNull();
                c.DisplayName.ShouldBe(catalogProduct.DisplayName);
                c.ProductId.ShouldBe(catalogProduct.ProductId);
            });
        });
    }

    [Fact(DisplayName = "Should GetCatalogCategoryDetail With Search CatalogProduct Correctly")]
    public async Task Should_GetCatalogCategoryDetail_With_Search_CatalogProduct_Correctly()
    {
        var catalogCategory = this.CatalogCategory;
        var catalogCategoryId = catalogCategory.Id;

        var request = new GetCatalogCategoryDetailRequest
        {
            CatalogCategoryId = catalogCategoryId,
            CatalogProductCriteria = new GetCatalogCategoryDetailRequest.CatalogProductSearchRequest
            {
                SearchTerm = this.CatalogProduct.DisplayName
            }
        };

        await this.ExecuteTestRequestHandler<GetCatalogCategoryDetailRequest, GetCatalogCategoryDetailResult>(request, result =>
        {
            result.ShouldNotBeNull();
            result.CatalogCategoryDetail.ShouldNotBeNull();
            result.CatalogCategoryDetail.CatalogId.ShouldBe(catalogCategory.CatalogId);
            result.CatalogCategoryDetail.CatalogCategoryId.ShouldBe(catalogCategoryId);
            result.CatalogCategoryDetail.CatalogCategoryName.ShouldBe(catalogCategory.DisplayName);
            result.CatalogCategoryDetail.CatalogName.ShouldBe(this.Catalog.DisplayName);

            result.TotalOfCatalogProducts.ShouldBe(1);

            result.CatalogProducts.ToList().ForEach(c =>
            {
                var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x.Id == c.CatalogProductId);

                catalogProduct.ShouldNotBeNull();
                c.DisplayName.ShouldBe(catalogProduct.DisplayName);
                c.ProductId.ShouldBe(catalogProduct.ProductId);
            });
        });
    }

    [Fact(DisplayName = "Should Validate GetCatalogCategoryDetailRequest Correctly")]
    public async Task Should_Validate_GetCatalogCategoryDetailRequest_Correctly()
    {
        var request = new GetCatalogCategoryDetailRequest
        {
            CatalogCategoryId = CatalogCategoryId.Empty
        };

        await this.ExecuteValidationTest(request,
            result => { result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId); });
    }
}
