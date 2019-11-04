using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests
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
            services.AddEfCoreSqlServerDb();
            services.AddSingleton<IConfiguration>(sp => this.Configuration);
        }
    }
}
