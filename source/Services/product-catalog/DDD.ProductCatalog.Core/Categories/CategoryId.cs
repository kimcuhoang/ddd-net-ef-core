using System;
using DNK.DDD.Core.Models;

namespace DDD.ProductCatalog.Core.Categories
{
    public class CategoryId : IdentityBase
    {
        #region Constructors

        private CategoryId(Guid id) : base(id) { }

        #endregion

        public static CategoryId New => new CategoryId(Guid.NewGuid());
        public static CategoryId Of(Guid id) => new CategoryId(id);
        public static CategoryId Empty => new CategoryId(Guid.Empty);
    }
}
