using DDDEfCore.Core.Common.Models;
using System;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogProductId : IdentityBase
    {
        #region Constructors

        private CatalogProductId(Guid id) : base(id) { }

        #endregion

        public static explicit operator CatalogProductId(Guid id) => id == Guid.Empty ? null : new CatalogProductId(id);
    }
}
