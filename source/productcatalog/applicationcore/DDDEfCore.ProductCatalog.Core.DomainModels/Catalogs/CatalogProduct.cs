using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using System;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogProduct : EntityBase
    {
        public CatalogProductId CatalogProductId => (CatalogProductId) this.Id;

        public string DisplayName { get; private set; }

        public ProductId ProductId { get; private set; }

        public CatalogCategory CatalogCategory { get; private set; }

        #region Constructors

        private CatalogProduct(CatalogProductId catalogProductId, ProductId productId, string displayName, CatalogCategory catalogCategory)
            : base(catalogProductId)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new DomainException($"{nameof(displayName)} is empty.");

            this.ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
            this.CatalogCategory = catalogCategory ?? throw new ArgumentNullException(nameof(catalogCategory));

            this.DisplayName = displayName;
        }

        private CatalogProduct(ProductId productId, string displayName, CatalogCategory catalogCategory)
            : this(IdentityFactory.Create<CatalogProductId>(), productId, displayName, catalogCategory) { }

        private CatalogProduct() { }
        #endregion

        #region Creations

        internal static CatalogProduct Create(ProductId productId, string displayName, CatalogCategory catalogCategory)
            => new CatalogProduct(productId, displayName, catalogCategory);

        #endregion

        #region Behaviors

        public CatalogProduct ChangeDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new DomainException($"{nameof(displayName)} is empty.");

            this.DisplayName = displayName;

            return this;
        }

        #endregion
    }
}
