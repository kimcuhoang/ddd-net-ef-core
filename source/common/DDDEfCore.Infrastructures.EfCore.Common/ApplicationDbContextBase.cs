using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.Infrastructures.EfCore.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DDDEfCore.Infrastructures.EfCore.Common
{
    public abstract class ApplicationDbContextBase : DbContext
    {
        private readonly IConfiguration _configuration;

        protected ApplicationDbContextBase(DbContextOptions dbContextOptions, IConfiguration configuration)
            : base(dbContextOptions)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var qualifiedAssemblyPattern = this._configuration.GetValue<string>("QualifiedAssemblyPattern");

            var assemblies = AssemblyHelpers.LoadFromSearchPatterns(qualifiedAssemblyPattern);

            builder.Register(assemblies);

            this.RegisterConventions(builder);
        }

        protected virtual void RegisterConventions(ModelBuilder builder) { }
    }
}
