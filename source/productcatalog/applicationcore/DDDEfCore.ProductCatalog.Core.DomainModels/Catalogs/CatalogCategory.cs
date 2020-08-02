using System;
using System.Collections.Generic;
using System.Linq;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogCategory : EntityBase<CatalogCategoryId>
    {
        public CatalogId CatalogId { get; private set; }

        public CategoryId CategoryId { get; private set; }

        public string DisplayName { get; private set; }

        public CatalogCategory Parent { get; private set; }

        public bool IsRoot => Parent == null;

        private List<CatalogProduct> _products = new List<CatalogProduct>();

        public IEnumerable<CatalogProduct> Products => this._products;

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

        public CatalogCategory ChangeDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new DomainException($"{nameof(displayName)} is empty.");
            }

            this.DisplayName = displayName;

            return this;
        }

        #endregion

        #region Behaviors with CatalogProduct

        public CatalogProduct CreateCatalogProduct(ProductId productId, string displayName)
        {
            if (productId == null)
                throw new DomainException($"{nameof(productId)} is null.");

            if (this._products.Any(x => x.ProductId == productId))
                throw new DomainException($"Product#{productId} is existing in CatalogCategory#{this.Id}");

            var catalogProduct = CatalogProduct.Create(productId, displayName, this);

            this._products.Add(catalogProduct);

            return catalogProduct;
        }

        public void RemoveCatalogProduct(CatalogProduct catalogProduct)
        {
            if (catalogProduct == null)
                throw new DomainException($"{nameof(catalogProduct)} is null");

            this._products = _products.Where(x => x != catalogProduct).ToList();
        }

        #endregion
    }
}
