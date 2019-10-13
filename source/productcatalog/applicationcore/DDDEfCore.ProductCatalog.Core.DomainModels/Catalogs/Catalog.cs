using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class Catalog : AggregateRoot
    {
        public CatalogId CatalogId => (CatalogId)this.Id;

        public string DisplayName { get; private set; }

        private List<CatalogCategory> _categories = new List<CatalogCategory>();

        public IEnumerable<CatalogCategory> Categories => this._categories;

        #region Constructors

        private Catalog(CatalogId catalogId, string catalogName) : base(catalogId)
        {
            if (string.IsNullOrWhiteSpace(catalogName))
            {
                throw new DomainException($"{nameof(catalogName)} is empty.");
            }

            this.DisplayName = catalogName;
        }

        private Catalog(string catalogName) : this(IdentityFactory.Create<CatalogId>(), catalogName) { }

        private Catalog() { }

        #endregion

        #region Creations

        public static Catalog Create(string catalogName) => new Catalog(catalogName);

        #endregion

        #region Behaviors

        public Catalog ChangeDisplayName(string catalogName)
        {
            if (string.IsNullOrWhiteSpace(catalogName))
            {
                throw new DomainException($"{nameof(catalogName)} is empty.");
            }

            this.DisplayName = catalogName;
            return this;
        }

        #endregion

        #region Behaviors with CatalogCategory

        public CatalogCategory AddCategory(CategoryId categoryId, string displayName, CatalogCategory parentCatalogCategory = null)
        {
            if (categoryId == null)
                throw new DomainException($"{nameof(categoryId)} is null");

            if (this._categories.Any(x => x.Parent == parentCatalogCategory && x.CategoryId == categoryId))
                throw new DomainException($"Category#{categoryId} is existing in Catalog#{this.CatalogId}");

            var catalogCategory =
                CatalogCategory.Create(this.CatalogId, categoryId, displayName, parentCatalogCategory);

            this._categories.Add(catalogCategory);

            return catalogCategory;
        }

        public IEnumerable<CatalogCategory> FindCatalogCategoryRoots()
            => this._categories.Where(x => x.Parent == null);

        public IEnumerable<CatalogCategory> GetDescendantsOfCatalogCategory(CatalogCategory catalogCategory)
        {
            if (catalogCategory == null)
                throw new DomainException($"{nameof(catalogCategory)} is null.");
            
            var descendants = new List<CatalogCategory>();

            this.GetDescendants(catalogCategory, descendants);

            return descendants;
        }

        private void GetDescendants(CatalogCategory catalogCategory, List<CatalogCategory> descendants)
        {
            var children = this._categories.Where(x => x.Parent == catalogCategory);

            foreach (var kid in children)
            {
                GetDescendants(kid, descendants);
            }

            descendants.Add(catalogCategory);
        }

        public void RemoveCatalogCategoryWithDescendants(CatalogCategory catalogCategory)
        {
            if (catalogCategory == null)
                throw new DomainException($"{nameof(catalogCategory)} is null.");

            var descendants = this.GetDescendantsOfCatalogCategory(catalogCategory);

            foreach (var kid in descendants)
            {
                this._categories = this._categories.Where(x => x != kid).ToList();
            }
        }

        #endregion
    }
}
