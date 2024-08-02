using DNK.DDD.Infrastructure.EntityFrameworkCore.MsSqlServer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DDD.ProductCatalog.Infrastructure.EfCore;
public class ProductCatalogDbContext(DbContextOptions options) : MsSqlServerDbContext(options)
{
    public override Assembly AssemblyContainsConfigurations => Assembly.GetExecutingAssembly();
}
