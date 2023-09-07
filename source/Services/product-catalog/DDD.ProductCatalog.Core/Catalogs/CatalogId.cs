using System;
using DNK.DDD.Core.Models;

namespace DDD.ProductCatalog.Core.Catalogs
{
    public class CatalogId : IdentityBase
    {
        #region Constructors

        private CatalogId(Guid id) : base(id) { }

        #endregion

        public static CatalogId New => new CatalogId(Guid.NewGuid());

        public static CatalogId Of(Guid id) => new CatalogId(id);

        public static CatalogId Empty => new CatalogId(Guid.Empty);
    }
}
