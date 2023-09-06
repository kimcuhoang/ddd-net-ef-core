using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;

public class CatalogProduct : EntityBase<CatalogProductId>
{
    public string DisplayName { get; private set; }

    public ProductId ProductId { get; private set; }

    public CatalogCategory CatalogCategory { get; private set; }

    #region Constructors

    private CatalogProduct(CatalogProductId id, string displayName, ProductId productId) : base(id)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException($"{nameof(displayName)} is empty.");
        }
        this.DisplayName = displayName;

        this.ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
    }

    #endregion

    #region Creations

    internal static CatalogProduct Create(ProductId productId, string displayName, CatalogCategory catalogCategory)
        => new(CatalogProductId.New, displayName, productId)
        {
            CatalogCategory = catalogCategory
        };

    #endregion

    #region Behaviors

    public CatalogProduct ChangeDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException($"{nameof(displayName)} is empty.");
        }

        this.DisplayName = displayName;

        return this;
    }

    #endregion
}
