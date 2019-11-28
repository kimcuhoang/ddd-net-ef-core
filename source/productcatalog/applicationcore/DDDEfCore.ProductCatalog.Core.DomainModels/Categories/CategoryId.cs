using System;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Categories
{
    public class CategoryId : IdentityBase
    {
        #region Constructors

        private CategoryId(Guid id) : base(id) { }

        #endregion

        public static explicit operator CategoryId(Guid id) => id == Guid.Empty ? null : new CategoryId(id);
    }
}
