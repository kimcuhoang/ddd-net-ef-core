using DNK.DDD.Infrastructure.EntityFrameworkCore.MsSqlServer.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DNK.DDD.Infrastructure.EntityFrameworkCore.MsSqlServer;

public abstract class MsSqlServerDbContext : DbContextBase
{
    protected MsSqlServerDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void InnerConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.InnerConfigureConventions(configurationBuilder);

        configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
        configurationBuilder.Conventions.Add(_ => new TableNameConvention());
    }
}
