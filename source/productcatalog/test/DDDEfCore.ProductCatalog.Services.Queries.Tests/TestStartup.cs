using DDDEfCore.ProductCatalog.Infrastructure.EfCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests
{
    public sealed class TestStartup
    {
        public IConfiguration Configuration { get; }

        public TestStartup(IWebHostEnvironment env)
        {
            this.Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(sp => this.Configuration);
            services.AddEfCoreSqlServerDb();
            services.AddApplicationQueries();
        }
    }
}
