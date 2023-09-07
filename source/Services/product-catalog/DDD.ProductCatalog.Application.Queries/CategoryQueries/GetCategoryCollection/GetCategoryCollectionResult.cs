using DDD.ProductCatalog.Core.Categories;

namespace DDD.ProductCatalog.Application.Queries.CategoryQueries.GetCategoryCollection;

public class GetCategoryCollectionResult
{
    public int TotalCategories { get; set; }
    public IEnumerable<CategoryResult> Categories { get; set; }

    public class CategoryResult
    {
        public CategoryId Id { get; set; }
        public string DisplayName { get; set; }
    }
}
