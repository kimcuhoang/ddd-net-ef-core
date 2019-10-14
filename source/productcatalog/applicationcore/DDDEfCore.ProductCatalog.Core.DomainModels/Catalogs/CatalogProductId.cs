using DDDEfCore.Core.Common.Models;
using System;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogProductId : IdentityBase
    {
        #region Constructors

        public CatalogProductId(Guid id) : base(id) { }

        public CatalogProductId() : base() { }

        #endregion
    }
}
