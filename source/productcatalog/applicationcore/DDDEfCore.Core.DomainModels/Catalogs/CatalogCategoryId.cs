using DDDEfCore.Core.Common.Models;
using System;

namespace DDDEfCore.Core.DomainModels.Catalogs
{
    public class CatalogCategoryId : IdentityBase
    {
        #region Constructors

        public CatalogCategoryId(Guid id) : base(id) { }

        private CatalogCategoryId() { }

        #endregion
    }
}
