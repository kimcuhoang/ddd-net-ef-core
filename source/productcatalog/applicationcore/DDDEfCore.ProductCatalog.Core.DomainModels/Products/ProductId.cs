using System;
using System.Collections.Generic;
using System.Text;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Products
{
    public class ProductId : IdentityBase
    {
        #region Constructors

        public ProductId(Guid id) : base(id) { }

        public ProductId() : base() { }

        #endregion
    }
}
