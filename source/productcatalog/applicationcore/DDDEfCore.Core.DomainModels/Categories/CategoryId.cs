using DDDEfCore.Core.Common.Models;
using System;

namespace DDDEfCore.Core.DomainModels.Categories
{
    public class CategoryId : IdentityBase
    {
        #region Constructors

        public CategoryId(Guid id) : base(id) { }

        private CategoryId() { }

        #endregion

        public static CategoryId New() => new CategoryId();
    }
}
