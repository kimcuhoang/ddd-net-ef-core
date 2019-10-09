using System.Linq;
using DDDEfCore.Infrastructures.EfCore.Common;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Db
{
    public class ProductCatalogDbContext : ApplicationDbContextBase
    {
        public ProductCatalogDbContext(DbContextOptions dbContextOptions, IConfiguration configuration) 
            : base(dbContextOptions, configuration)
        {
        }

        #region Overrides of ApplicationDbContextBase

        protected override void RegisterConventions(ModelBuilder builder)
        {
            //base.RegisterConventions(builder);

            //var types = builder.Model
            //    .GetEntityTypes()
            //    .Where(entity => !string.IsNullOrWhiteSpace(entity.ClrType.Namespace));

            //foreach (var entityType in types)
            //{
            //    builder.Entity(entityType.Name).ToTable(entityType.ClrType.Namespace.Pluralize());
            //}
        }

        #endregion
    }
}
