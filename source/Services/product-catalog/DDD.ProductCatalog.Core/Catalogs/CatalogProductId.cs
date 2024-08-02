using DNK.DDD.Core.Models;

namespace DDD.ProductCatalog.Core.Catalogs;

public class CatalogProductId : IdentityBase
{
    #region Constructors

    private CatalogProductId(Guid id) : base(id) { }

    #endregion

    public static CatalogProductId New => new(Guid.NewGuid());
    public static CatalogProductId Of(Guid id) => new(id);
    public static CatalogProductId Empty => new(Guid.Empty);
}
