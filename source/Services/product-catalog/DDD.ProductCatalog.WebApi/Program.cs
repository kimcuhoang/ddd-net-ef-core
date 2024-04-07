using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using DDD.ProductCatalog.Infrastructure;
using DDD.ProductCatalog.WebApi.Infrastructures.Middlewares;
using DDD.ProductCatalog.WebApi.Infrastructures;
using DDD.ProductCatalog.WebApi.Infrastructures.HostedServices;
using DDD.ProductCatalog.WebApi.Infrastructures.JsonConverters;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.AddProductCatalogInfrastructure("DefaultDb");

// Configure for ControllerBase
services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.AllowTrailingCommas = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new IdentityJsonConverterFactory());
});

// Configure for MinimalApi
services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.AllowTrailingCommas = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new IdentityJsonConverterFactory());
});

services.AddSwaggerConfig();

services.AddHostedService<DbMigratorHostedService>();

builder.Host
    .UseDefaultServiceProvider((context, options) =>
    {
        options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
        options.ValidateOnBuild = true;
    });

var app = builder.Build();
var env = builder.Environment;

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseAuthorization();

app.UseSwaggerMiddleware();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseRouting();
app.MapControllers();

app.MapGet("/", () => TypedResults.Redirect("/swagger", permanent: true));

await app.RunAsync();

public partial class Program { }
