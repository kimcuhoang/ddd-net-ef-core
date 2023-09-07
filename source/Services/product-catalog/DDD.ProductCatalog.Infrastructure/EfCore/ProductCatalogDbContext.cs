using DNK.DDD.Infrastructure.EntityFrameworkCore.MsSqlServer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DDD.ProductCatalog.Infrastructure.EfCore;
public class ProductCatalogDbContext : MsSqlServerDbContext
{
    public ProductCatalogDbContext(DbContextOptions options) : base(options)
    {
    }

    public override Assembly AssemblyContainsConfigurations => Assembly.GetExecutingAssembly();
}
