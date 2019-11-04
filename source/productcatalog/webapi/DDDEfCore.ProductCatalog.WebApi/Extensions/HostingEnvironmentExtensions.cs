using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DDDEfCore.ProductCatalog.WebApi.Extensions
{
    public static class HostingEnvironmentExtensions
    {
        public static IConfigurationRoot GetConfiguration(this IWebHostEnvironment hostingEnvironment)
        {
            var basePath = hostingEnvironment.ContentRootPath;
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
