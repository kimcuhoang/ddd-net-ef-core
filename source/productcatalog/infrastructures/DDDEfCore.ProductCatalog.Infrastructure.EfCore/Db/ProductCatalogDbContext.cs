using DDDEfCore.Infrastructures.EfCore.Common;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore.Db.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Reflection;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Db;

public class ProductCatalogDbContext : ApplicationDbContextBase
{
    public ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> dbContextOptions) 
        : base(dbContextOptions)
    {
    }

    protected override Assembly AssemblyContainsConfigurations => Assembly.GetExecutingAssembly();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
        configurationBuilder.Conventions.Add(_ => new TableNameConvention());
    }
}
