using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Products
{
    public class Product : AggregateRoot<ProductId>
    {
        public string Name { get; private set; }

        #region Constructors

        private Product(ProductId id, string productName) : base(id)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new DomainException($"{nameof(productName)} is empty.");
            }

            this.Name = productName;
        }

        private Product(string productName) : this(IdentityFactory.Create<ProductId>(), productName) { }

        private Product() { }
        #endregion

        #region Creations

        public static Product Create(string productName) => new Product(productName);

        #endregion

        #region Behaviors

        public Product ChangeName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new DomainException($"{nameof(productName)} is empty.");
            }

            this.Name = productName;
            return this;
        }

        #endregion
    }
}
