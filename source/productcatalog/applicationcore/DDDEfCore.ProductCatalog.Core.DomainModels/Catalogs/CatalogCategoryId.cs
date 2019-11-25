using System;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogCategoryId : IdentityBase
    {
        #region Constructors

        private CatalogCategoryId(Guid id) : base(id) { }

        #endregion

        public static explicit operator CatalogCategoryId(Guid id) => id == Guid.Empty ? null : new CatalogCategoryId(id);
    }
}
