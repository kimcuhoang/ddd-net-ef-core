using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Tests.TestCategory
{
    public class TestCategoryCreation
    {
        [Theory(DisplayName = "Create Category with Display Name Successfully")]
        [AutoData]
        public void CreateCategory_WithDisplayName_Successfully(string categoryName)
        {
            var category = Category.Create(categoryName);

            category.ShouldNotBeNull();
            category.CategoryId.ShouldNotBeNull();
            category.DisplayName.ShouldBe(categoryName);
        }

        [Fact(DisplayName = "Create Category without Display Name Should Throw Exception")]
        public void CreateCategory_WithoutDisplayName_ShouldThrowException()
        {
            Should.Throw<DomainException>(() => Category.Create(string.Empty));
        }
    }
}
