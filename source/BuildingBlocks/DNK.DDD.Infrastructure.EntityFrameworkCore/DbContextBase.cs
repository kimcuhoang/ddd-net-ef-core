using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DNK.DDD.Infrastructure.EntityFrameworkCore;
public abstract class DbContextBase(DbContextOptions options) : DbContext(options)
{
    public abstract Assembly AssemblyContainsConfigurations {  get; }

    protected virtual void SetDefaultSchema(ModelBuilder modelBuilder) { }

    protected virtual void InnerConfigureConventions(ModelConfigurationBuilder configurationBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        this.SetDefaultSchema(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(this.AssemblyContainsConfigurations);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        this.InnerConfigureConventions(configurationBuilder);
    }
}
