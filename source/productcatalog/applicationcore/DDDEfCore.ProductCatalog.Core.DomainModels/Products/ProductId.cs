using DDDEfCore.Core.Common.Models;
using System;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Products
{
    public class ProductId : IdentityBase
    {
        #region Constructors

        private ProductId(Guid id) : base(id) { }

        #endregion

        public static ProductId New => new ProductId(Guid.NewGuid());
        public static ProductId Of(Guid id) => new ProductId(id);
        public static ProductId Empty => new ProductId(Guid.Empty);
    }
}
