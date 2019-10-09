using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Core.DomainModels.Categories;
using DDDEfCore.Core.DomainModels.Exceptions;

namespace DDDEfCore.Core.DomainModels.Catalogs
{
    public class Catalog : AggregateRoot
    {
        public CatalogId CatalogId => (CatalogId)this.Id;

        public string DisplayName { get; private set; }

        private List<CatalogCategory> _categories = new List<CatalogCategory>();

        public IEnumerable<CatalogCategory> Categories => this._categories.AsReadOnly();

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

        public bool HasCategory(CategoryId categoryId)
            => this._categories.Any(x => x.CategoryId == categoryId);

        public CatalogCategory AddCategoryRoot(CategoryId categoryId)
        {
            if (categoryId == null)
            {
                throw new DomainException($"{nameof(categoryId)} is null.");
            }

            if (this.HasCategory(categoryId))
            {
                throw new DomainException($"{categoryId} is existing in {this.CatalogId}");
            }

            var catalogCategory = CatalogCategory.Create(this.CatalogId, categoryId);

            this._categories.Add(catalogCategory);

            return catalogCategory;
        }

        public void RemoveCategoryWithDescendants(CategoryId categoryId)
        {
            if (categoryId == null)
            {
                throw new DomainException($"{nameof(categoryId)} is null.");
            }

            var catalogCategory = this._categories.FirstOrDefault(x => x.CategoryId == categoryId);

            if (catalogCategory == null)
            {
                throw new DomainException($"Could not found {categoryId} in {this.CatalogId}");
            }

            var relatedCategories = catalogCategory.GetDescendants();

            foreach (var category in relatedCategories)
            {
                this._categories.Remove(category);
            }

            this._categories.Remove(catalogCategory);
        }

        #endregion
    }
}
