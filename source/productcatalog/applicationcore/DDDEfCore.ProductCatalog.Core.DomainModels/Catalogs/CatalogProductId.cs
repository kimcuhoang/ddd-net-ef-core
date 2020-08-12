using DDDEfCore.Core.Common.Models;
using System;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogProductId : IdentityBase
    {
        #region Constructors

        private CatalogProductId(Guid id) : base(id) { }

        #endregion

        public static CatalogProductId New => new CatalogProductId(Guid.NewGuid());
        public static CatalogProductId Of(Guid id) => new CatalogProductId(id);
        public static CatalogProductId Empty => new CatalogProductId(Guid.Empty);
    }
}
