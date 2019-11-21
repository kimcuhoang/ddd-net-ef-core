using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using System.Collections.Generic;
using System.Linq;

namespace DDDEfCore.ProductCatalog.Services.Queries.CategoryQueries.GetCategoryDetail
{
    public class GetCategoryDetailResult
    {
        public CategoryDetailResult CategoryDetail { get; set; }

        public IEnumerable<CatalogOfCategoryResult> AssignedToCatalogs { get; set; }

        public int TotalCatalogs => AssignedToCatalogs?.Count() ?? 0;

        public class CategoryDetailResult
        {
            public CategoryId Id { get; set; }
            public string DisplayName { get; set; }
        }

        public class CatalogOfCategoryResult
        {
            public CatalogId Id { get; set; }
            public string DisplayName { get; set; }
        }
    }
}
