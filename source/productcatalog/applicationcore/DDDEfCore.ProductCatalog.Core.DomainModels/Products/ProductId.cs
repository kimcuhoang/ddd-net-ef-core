using DDDEfCore.Core.Common.Models;
using System;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Products
{
    public class ProductId : IdentityBase
    {
        #region Constructors

        public ProductId(Guid id) : base(id) { }

        public ProductId() : base() { }

        #endregion

        public static explicit operator ProductId(Guid id) => new ProductId(id);
    }
}
