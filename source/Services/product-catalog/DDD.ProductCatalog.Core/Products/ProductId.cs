using DNK.DDD.Core.Models;

namespace DDD.ProductCatalog.Core.Products;

public class ProductId : IdentityBase
{
    #region Constructors

    private ProductId(Guid id) : base(id) { }

    #endregion

    public static ProductId New => new(Guid.NewGuid());
    public static ProductId Of(Guid id) => new(id);
    public static ProductId Empty => new(Guid.Empty);
}
