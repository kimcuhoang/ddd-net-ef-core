using DNK.DDD.Core.Models;
using DDD.ProductCatalog.Core.Exceptions;

namespace DDD.ProductCatalog.Core.Products;

public class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; }

    #region Constructors

    private Product(ProductId id, string name) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException($"{nameof(name)} is empty.");
        }

        this.Name = name;
    }

    private Product(string name) : this(ProductId.New, name) { }

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
