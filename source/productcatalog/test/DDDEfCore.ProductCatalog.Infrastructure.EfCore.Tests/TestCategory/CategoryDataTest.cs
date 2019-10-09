using System.Collections.Generic;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCategory
{
    public class CategoryDataTest
    {
        public static IEnumerable<object[]> NewCategory =>
            new List<object[]>
            {
                new object[] { Category.Create("The new category") }
            };

        public static IEnumerable<object[]> UpdateCategory =>
            new List<object[]>
            {
                new object[] { Category.Create("The new category"), "Should change to new name" }
            };
    }
}
