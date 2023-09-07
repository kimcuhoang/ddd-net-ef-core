using DDD.ProductCatalog.Core.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Configurations;

public class ProductTypeConfiguration : IEntityTypeConfiguration<Product>
{
    #region Implementation of IEntityTypeConfiguration<Product>

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(_ => _.Id);

        builder
            .Property(x => x.Id)
            .HasConversion(x => x.Id, id => ProductId.Of(id));
    }

    #endregion
}
