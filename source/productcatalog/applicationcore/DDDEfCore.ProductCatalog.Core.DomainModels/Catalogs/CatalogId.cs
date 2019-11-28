using System;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogId : IdentityBase
    {
        #region Constructors

        private CatalogId(Guid id) : base(id) { }

        #endregion

        public static explicit operator CatalogId(Guid id) => id == Guid.Empty ? null : new CatalogId(id);
    }
}
