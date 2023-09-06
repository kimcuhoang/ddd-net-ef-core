namespace DDDEfCore.ProductCatalog.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configurationBuilder.AddJsonFile("appsettings.json");
                    configurationBuilder.AddJsonFile($"appsettings.{webHostBuilderContext.HostingEnvironment.EnvironmentName}.json", true);
                    configurationBuilder.AddEnvironmentVariables();
                });
                webBuilder.CaptureStartupErrors(true);
            })// Add a new service provider configuration
            .UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                options.ValidateOnBuild = true;
            });
}
