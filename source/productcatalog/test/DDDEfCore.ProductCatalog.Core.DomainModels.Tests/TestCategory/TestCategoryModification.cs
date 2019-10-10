using AutoFixture.Xunit2;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using Shouldly;
using Xunit;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Tests.TestCategory
{
    public class TestCategoryModification
    {
        [Theory(DisplayName = "Modify Category By Changing Name Successfully")]
        [AutoData]
        public void ModifyCategory_ByChangingName_Successfully(string originName, string changeWithName)
        {
            var category = Category.Create(originName);
            category.ChangeDisplayName(changeWithName);

            category.DisplayName.ShouldBe(changeWithName);
        }

        [Fact(DisplayName = "Set Empty to Category's Display Name Should Throw Exception")]
        public void ModifyCategory_SetEmptyDisplayName_ThrowException()
        {
            var category = Category.Create("Category");
            Should.Throw<DomainException>(() => category.ChangeDisplayName(string.Empty));
        }
    }
}
