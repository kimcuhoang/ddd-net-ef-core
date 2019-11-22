using System;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogCategoryId : IdentityBase
    {
        #region Constructors

        public CatalogCategoryId(Guid id) : base(id) { }

        public CatalogCategoryId() : base() { }

        #endregion

        public static explicit operator CatalogCategoryId(Guid id) => id == Guid.Empty ? null : new CatalogCategoryId(id);
    }
}
