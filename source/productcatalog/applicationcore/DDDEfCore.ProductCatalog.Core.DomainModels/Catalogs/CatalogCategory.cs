using System;
using System.Collections.Generic;
using System.Linq;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogCategory : EntityBase
    {
        public CatalogCategoryId CatalogCategoryId => (CatalogCategoryId) this.Id;

        public CatalogId CatalogId { get; private set; }

        public CategoryId CategoryId { get; private set; }

        public string DisplayName { get; private set; }

        public CatalogCategory Parent { get; private set; }

        public bool IsRoot => Parent == null;

        #region Constructors

        private CatalogCategory(CatalogCategoryId catalogCategoryId, CatalogId catalogId, CategoryId categoryId, string displayName, CatalogCategory parent = null)
            : base(catalogCategoryId)
        {
            this.CatalogId = catalogId ?? throw new ArgumentNullException(nameof(catalogId));
            this.CategoryId = categoryId ?? throw new ArgumentNullException(nameof(categoryId));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new DomainException($"{nameof(displayName)} is empty.");

            this.DisplayName = displayName;
            this.Parent = parent;
        }

        private CatalogCategory(CatalogId catalogId, CategoryId categoryId, string displayName, CatalogCategory parent = null)
            : this(IdentityFactory.Create<CatalogCategoryId>(), catalogId, categoryId, displayName, parent) { }

        private CatalogCategory() { }

        #endregion

        #region Creations

        internal static CatalogCategory Create(CatalogId catalogId, CategoryId categoryId, string displayName, CatalogCategory parent = null)
            => new CatalogCategory(catalogId, categoryId, displayName, parent);

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

        #endregion
    }
}
