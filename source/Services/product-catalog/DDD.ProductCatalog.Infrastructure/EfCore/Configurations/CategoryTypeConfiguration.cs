using DDD.ProductCatalog.Core.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD.ProductCatalog.Infrastructure.EfCore.Configurations;

public class CategoryTypeConfiguration : IEntityTypeConfiguration<Category>
{
    #region Implementation of IEntityTypeConfiguration<Category>

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(_ => _.Id);

        builder
            .Property(x => x.Id)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(x => x.Id, id => CategoryId.Of(id));
    }

    #endregion
}
