using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Core.DomainModels.Categories;
using DDDEfCore.Core.DomainModels.Exceptions;

namespace DDDEfCore.Core.DomainModels.Catalogs
{
    public class CatalogCategory : EntityBase
    {
        public CatalogCategoryId CatalogCategoryId => (CatalogCategoryId) this.Id;

        public CatalogId CatalogId { get; private set; }

        public CategoryId CategoryId { get; private set; }

        public string DisplayName { get; private set; }

        public DateTime AvailableFromDate { get; private set; } = DateTime.UtcNow;

        public DateTime? AvailableToDate { get; private set; }

        public CatalogCategory Parent { get; private set; }

        private List<CatalogCategory> _subCategories = new List<CatalogCategory>();

        public IEnumerable<CatalogCategory> SubCategories => this._subCategories.AsReadOnly();

        #region Constructors

        private CatalogCategory(CatalogCategoryId catalogCategoryId, CatalogId catalogId, CategoryId categoryId)
            : base(catalogCategoryId)
        {
            this.CatalogId = catalogId ?? throw new ArgumentNullException(nameof(catalogId));
            this.CategoryId = categoryId ?? throw new ArgumentNullException(nameof(categoryId));
        }

        private CatalogCategory(CatalogId catalogId, CategoryId categoryId)
            : this(IdentityFactory.Create<CatalogCategoryId>(), catalogId, categoryId) { }

        #endregion

        #region Creations

        public static CatalogCategory Create(CatalogId catalogId, CategoryId categoryId)
            => new CatalogCategory(catalogId, categoryId);

        #endregion

        #region Behaviors

        public CatalogCategory WithDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new DomainException($"{nameof(displayName)} is empty.");
            }

            this.DisplayName = displayName;

            return this;
        }

        public CatalogCategory SetAvailableDuration(DateTime availableFromDate, DateTime? availableToDate = null)
        {
            if (availableFromDate == null)
            {
                throw new DomainException($"{nameof(availableFromDate)} is null.");
            }

            this.AvailableFromDate = availableFromDate;

            this.AvailableToDate = availableToDate;

            return this;
        }

        internal CatalogCategory SetParent(CatalogCategory parent)
        {
            this.Parent = parent;

            return this;
        }

        public bool HasCategory(CategoryId categoryId)
            => this._subCategories.Any(x => x.CategoryId == categoryId);

        public CatalogCategory AddSubCategory(CategoryId categoryId)
        {
            if (categoryId == null)
            {
                throw new DomainException($"{nameof(categoryId)} is null.");
            }

            if (this.HasCategory(categoryId))
            {
                throw new DomainException($"{categoryId} is existing in {this.CatalogCategoryId}");
            }

            var catalogCategory = new CatalogCategory(this.CatalogId, categoryId);
            catalogCategory.SetParent(this);

            this._subCategories.Add(catalogCategory);

            return catalogCategory;
        }

        public IEnumerable<CatalogCategory> GetDescendants()
        {
            var descendants = new List<CatalogCategory>();

            this.GetDescendantRecursive(this, descendants);

            return descendants;
        }

        private void GetDescendantRecursive(CatalogCategory current, List<CatalogCategory> descendants)
        {
            foreach (var category in current.SubCategories)
            {
                GetDescendantRecursive(category, descendants);
            }

            // At the leaf
            descendants.Add(current);
        }

        #endregion
    }
}
