using System;
using System.Collections.Generic;
using System.Text;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.Core.DomainModels.Catalogs
{
    public class CatalogId : IdentityBase
    {
        #region Constructors

        public CatalogId(Guid id) : base(id) { }

        private CatalogId() { }

        #endregion

        public static CatalogId New() => new CatalogId();
    }
}
