using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DDDEfCore.Infrastructures.EfCore.Common;

public abstract class ApplicationDbContextBase : DbContext
{
    protected ApplicationDbContextBase(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(this.AssemblyContainsConfigurations);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }

    protected abstract Assembly AssemblyContainsConfigurations { get; }

    protected virtual void RegisterDefaultSchema() { }
}
