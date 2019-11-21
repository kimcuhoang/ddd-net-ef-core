using System;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Categories
{
    public class CategoryId : IdentityBase
    {
        #region Constructors

        public CategoryId(Guid id) : base(id) { }

        public CategoryId() : base() { }

        #endregion

        public static explicit operator CategoryId(Guid id) => new CategoryId(id);
    }
}
