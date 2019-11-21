using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryDetail;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests.TestCategoryQueries
{
    [Collection(nameof(SharedFixture))]
    public class TestGetCategoryDetail : IClassFixture<TestGetCategoryFixture>
    {
        private readonly TestGetCategoryFixture _testFixture;
        private readonly CancellationToken _cancellationToken;
        private readonly GetCategoryDetailRequestValidator _validator;

        public TestGetCategoryDetail(TestGetCategoryFixture testFixture)
        {
            this._testFixture = testFixture;
            this._cancellationToken = new CancellationToken(false);
            this._validator = new GetCategoryDetailRequestValidator();
        }

        private Category _category;

        private Category Category
        {
            get
            {
                if (this._category == null)
                {
                    var randomNumber = GenFu.GenFu.Random.Next(0, this._testFixture.Categories.Count);
                    this._category = this._testFixture.Categories[randomNumber];
                }

                return this._category;
            }
        }

        private async Task ExecuteTestAndAssert(GetCategoryDetailRequest request, Action<GetCategoryDetailResult> assertFor)
        {
            await this._testFixture.ExecuteScopeAsync(async sqlConnection =>
            {
                IRequestHandler<GetCategoryDetailRequest, GetCategoryDetailResult> requestHandler =
                    new RequestHandler(sqlConnection, this._validator);

                var result = await requestHandler.Handle(request, this._cancellationToken);

                assertFor(result);
            });
        }

        [Fact(DisplayName = "Should get CategoryDetail within assigned Catalogs Correctly")]
        public async Task Should_Get_Category_Within_AssignedCatalogs_Correctly()
        {
            var request = new GetCategoryDetailRequest
            {
                CategoryId = this.Category.CategoryId
            };

            await this.ExecuteTestAndAssert(request, result =>
            {
                var predefinedCatalog = this._testFixture.Catalog;

                result.ShouldNotBeNull();
                result.CategoryDetail.Id.ShouldBe(this.Category.CategoryId);
                result.CategoryDetail.DisplayName.ShouldBe(this.Category.DisplayName);
                result.TotalCatalogs.ShouldBe(1);
                result.AssignedToCatalogs.ToList().ForEach(catalog =>
                {
                    catalog.Id.ShouldBe(predefinedCatalog.CatalogId);
                    catalog.DisplayName.ShouldBe(predefinedCatalog.DisplayName);
                });
            });
        }

        [Fact(DisplayName = "Not found CategoryDetail Should Not Throw Exception")]
        public async Task NotFound_Category_Should_Not_ThrowException()
        {
            var request = new GetCategoryDetailRequest
            {
                CategoryId = (CategoryId)Guid.NewGuid()
            };

            await this.ExecuteTestAndAssert(request, result =>
            {
                result.ShouldNotBeNull();
                result.CategoryDetail.Id.ShouldBeNull();
                result.CategoryDetail.DisplayName.ShouldBeNullOrWhiteSpace();
                result.TotalCatalogs.ShouldBe(0);
            });
        }

        [Fact(DisplayName = "Invalid CategoryId Should Throw ValidationException")]
        public async Task Invalid_CategoryId_Should_Throw_ValidationException()
        {
            var request = new GetCategoryDetailRequest
            {
                CategoryId = (CategoryId)Guid.Empty
            };

            await Should.ThrowAsync<ValidationException>(async () => await this.ExecuteTestAndAssert(request, result => { }));
        }

        [Fact(DisplayName = "Invalid CategoryId Should Be Fail For Validation")]
        public void Invalid_CategoryId_ShouldBe_Fail_For_Validation()
        {
            var request = new GetCategoryDetailRequest
            {
                CategoryId = (CategoryId)Guid.Empty
            };

            var validationResult = this._validator.TestValidate(request);

            validationResult.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }
    }
}
