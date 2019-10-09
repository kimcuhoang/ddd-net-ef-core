using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Mappings
{
    public class CategoryMappingConfiguration : IEntityTypeConfiguration<Category>
    {
        #region Implementation of IEntityTypeConfiguration<Category>

        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.CategoryId);

            builder
                .Property(x => x.CategoryId)
                .HasField("Id")
                .HasColumnName("Id")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(x => x.Id, id => new CategoryId(id));
        }

        #endregion
    }
}
