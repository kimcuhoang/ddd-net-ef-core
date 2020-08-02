using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Mappings
{
    public class CatalogMappingConfiguration : IEntityTypeConfiguration<Catalog>
    {
        #region Implementation of IEntityTypeConfiguration<Catalog>

        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
            builder
                .Property(x => x.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasConversion(x => x.Id, id => (CatalogId)id);

            builder
                .HasMany(x => x.Categories)
                .WithOne()
                .HasForeignKey(x => x.CatalogId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        #endregion
    }
}
