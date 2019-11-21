using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Services.Queries.CatalogQueries.GetCatalogDetail;
using FluentValidation;
using FluentValidation.TestHelper;
using GenFu;
using MediatR;
using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCatalogQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCatalogDetail : IClassFixture<TestGetCatalogFixture>
    {
        private readonly TestGetCatalogFixture _testFixture;
        private readonly CancellationToken _cancellationToken;
        private readonly GetCatalogDetailRequestValidator _validator;

        public TestGetCatalogDetail(TestGetCatalogFixture textFixture)
        {
            this._testFixture = textFixture ?? throw new ArgumentNullException(nameof(textFixture));
            this._cancellationToken = new CancellationToken(false);
            this._validator = new GetCatalogDetailRequestValidator();
        }

        private Catalog _catalog;
        private Catalog Catalog
        {
            get
            {
                if (this._catalog == null)
                {
                    while (true)
                    {
                        var randomNumber = GenFu.GenFu.Random.Next(0, this._testFixture.Catalogs.Count);
                        var randomCatalog = this._testFixture.Catalogs[randomNumber];
                        if (randomCatalog.Categories.Any())
                        {
                            this._catalog = randomCatalog;
                            break;
                        }
                    }
                }
                
                return this._catalog;
            }
        }

        private async Task ExecuteTestAndAssert(GetCatalogDetailRequest request, Action<GetCatalogDetailResult> assertFor)
        {
            await this._testFixture.ExecuteScopeAsync(async sqlConnection =>
            {
                IRequestHandler<GetCatalogDetailRequest, GetCatalogDetailResult> requestHandler =
                    new RequestHandler(sqlConnection, this._validator);

                var result = await requestHandler.Handle(request, this._cancellationToken);

                assertFor(result);
            });
        }

        [Theory(DisplayName = "Should GetCatalogDetail With Paging CatalogCategory Correctly")]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(1, int.MaxValue)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public async Task Should_GetCatalogDetail_With_Paging_CatalogCategory_Correctly(int pageIndex, int pageSize)
        {
            var catalogId = this.Catalog.CatalogId;

            var request = new GetCatalogDetailRequest(catalogId)
            {
                SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            };

            await this.ExecuteTestAndAssert(request, result =>
            {
                result.ShouldNotBeNull();
                result.CatalogDetail.ShouldSatisfyAllConditions(
                    () => result.CatalogDetail.Id.ShouldBe(catalogId),
                    () => result.CatalogDetail.DisplayName.ShouldBe(this.Catalog.DisplayName)
                );
                result.TotalOfCatalogCategories.ShouldBe(this.Catalog.Categories.Count());

                result.CatalogCategories.ToList().ForEach(c =>
                {
                    var catalogCategory =
                        this.Catalog.Categories.SingleOrDefault(x =>
                            x.CatalogCategoryId == new CatalogCategoryId(c.CatalogCategoryId));

                    catalogCategory.ShouldNotBeNull();
                    c.DisplayName.ShouldBe(catalogCategory.DisplayName);
                    c.TotalOfProducts.ShouldBe(catalogCategory.Products.Count());
                });
            });
        }

        [Fact(DisplayName = "Should GetCatalogDetail With Search CatalogCategory Correctly")]
        public async Task Should_GetCatalogDetail_With_Search_CatalogCategory_Correctly()
        {
            var catalogId = this.Catalog.CatalogId;
            var catalogCategories = this.Catalog.Categories.ToList();

            var randomIndex = A.Random.Next(0, catalogCategories.Count);
            var randomCatalogCategory = catalogCategories[randomIndex];

            var request = new GetCatalogDetailRequest(catalogId)
            {
                SearchCatalogCategoryRequest = new GetCatalogDetailRequest.CatalogCategorySearchRequest
                {
                    SearchTerm = randomCatalogCategory.DisplayName
                }
            };

            await this.ExecuteTestAndAssert(request, result =>
            {
                result.ShouldNotBeNull();
                result.CatalogDetail.ShouldSatisfyAllConditions(
                    () => result.CatalogDetail.Id.ShouldBe(catalogId),
                    () => result.CatalogDetail.DisplayName.ShouldBe(this.Catalog.DisplayName)
                );
                result.TotalOfCatalogCategories.ShouldBe(this.Catalog.Categories.Count());

                result.CatalogCategories.ToList().ForEach(c =>
                {
                    var catalogCategory =
                        this.Catalog.Categories.SingleOrDefault(x =>
                            x.CatalogCategoryId == new CatalogCategoryId(c.CatalogCategoryId));

                    catalogCategory.ShouldNotBeNull();
                    c.DisplayName.ShouldBe(catalogCategory.DisplayName);
                    c.TotalOfProducts.ShouldBe(catalogCategory.Products.Count());
                });
            });
        }

        [Fact(DisplayName = "Should GetCatalogDetail With Not Found Any CatalogCategory From Search Still Correct")]
        public async Task Should_GetCatalogDetail_With_NotFound_Any_CatalogCategory_From_Search_Still_Correct()
        {
            var catalogId = this._testFixture.CatalogWithoutCatalogCategory.CatalogId.Id;

            var request = new GetCatalogDetailRequest(catalogId);

            await this.ExecuteTestAndAssert(request, result =>
            {
                result.ShouldNotBeNull();
                result.TotalOfCatalogCategories.ShouldBe(0);
                result.CatalogCategories.ShouldBeEmpty();
            });
        }

        [Fact(DisplayName = "Should Throw Exception When Request Is Invalid")]
        public async Task Should_Throw_ValidationException_When_Request_Is_Invalid()
        {
            var request = new GetCatalogDetailRequest(Guid.Empty);

            await this._testFixture.ExecuteScopeAsync(async dbConnectionFactory =>
            {
                IRequestHandler<GetCatalogDetailRequest, GetCatalogDetailResult> requestHandler =
                    new RequestHandler(dbConnectionFactory, this._validator);

                await Should.ThrowAsync<ValidationException>(async () =>
                    await requestHandler.Handle(request, this._cancellationToken));
            });
        }

        [Fact(DisplayName = "Should GetCatalogDetail Successfully Even Not Found Catalog")]
        public async Task Should_GetCatalogDetail_Successfully_Even_NotFound_Catalog()
        {
            var request = new GetCatalogDetailRequest(Guid.NewGuid());

            await this.ExecuteTestAndAssert(request, result =>
            {
                result.ShouldNotBeNull();
                result.TotalOfCatalogCategories.ShouldBe(0);
                result.CatalogDetail.ShouldNotBeNull();
                result.CatalogDetail.Id.ShouldBeNull();
                result.CatalogDetail.DisplayName.ShouldBeNull();
                result.CatalogCategories.ShouldBeNull();
            });
        }

        [Fact(DisplayName = "Validation: GetCatalogDetail With Empty CatalogId Should Be Invalid")]
        public void GetCatalogDetail_With_Empty_CatalogId_ShouldBe_Invalid()
        {
            var request = new GetCatalogDetailRequest(Guid.Empty);

            var result = this._validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
        }
    }
}
