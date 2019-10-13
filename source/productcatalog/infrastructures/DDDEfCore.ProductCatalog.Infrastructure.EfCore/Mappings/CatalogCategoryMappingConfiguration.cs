using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Mappings
{
    public class CatalogCategoryMappingConfiguration : IEntityTypeConfiguration<CatalogCategory>
    {
        #region Implementation of IEntityTypeConfiguration<CatalogCategory>

        public void Configure(EntityTypeBuilder<CatalogCategory> builder)
        {
            builder.HasKey(x => x.CatalogCategoryId);

            builder
                .Property(x => x.CatalogCategoryId)
                .HasField("Id")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(x => x.Id, id => new CatalogCategoryId(id));

            builder
                .Property(x => x.CatalogId)
                .IsRequired()
                .HasConversion(x => x.Id, id => new CatalogId(id));

            builder
                .Property(x => x.CategoryId)
                .IsRequired()
                .HasConversion(x => x.Id, id => new CategoryId(id));

            builder
                .HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey("CatalogCategoryParentId")
                .IsRequired(false);

            builder.Ignore(x => x.IsRoot);
        }

        #endregion
    }
}
