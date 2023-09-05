using DDDEfCore.ProductCatalog.Services.Queries;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.HostedServices;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.JsonConverters;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.Middlewares;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore;
using DDDEfCore.ProductCatalog.Services.Commands;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore.MediatR.Pipelines;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.WebApi;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    private void ConfigureJsonSerializerOptions(JsonSerializerOptions options)
    {
        options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        options.PropertyNameCaseInsensitive = true;
        options.AllowTrailingCommas = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        options.Converters.Add(new IdentityJsonConverterFactory());
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(json =>
        {
            this.ConfigureJsonSerializerOptions(json.JsonSerializerOptions);
        });

        services.ConfigureHttpJsonOptions(json =>
        {
            this.ConfigureJsonSerializerOptions(json.SerializerOptions);
        });

        services.AddApplicationCommands();
        services.AddApplicationQueries();

        var assemblies = new[]
        {
            typeof(ITransactionCommand<>).Assembly,
            typeof(IRequestQuery<>).Assembly
        };

        services.AddEfCoreSqlServerDb(this.Configuration);
        services.AddMediatR(mediatR =>
        {
            mediatR
                .RegisterServicesFromAssemblies(assemblies)
                .AddOpenBehavior(typeof(RequestValidatorPipelineBehavior<,>))
                .AddOpenBehavior(typeof(CommandTransactionPipelineBehavior<,>));
        });

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddValidatorsFromAssemblies(assemblies);


        services.AddSwaggerConfig();
        services.AddHostedService<DbMigratorHostedService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        //app.UseAuthorization();

        app.UseSwaggerMiddleware();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}


/*
* Resources:
*  1. https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.0&tabs=visual-studio
*  2. Notes:
*      c.SwaggerDoc("V1", new OpenApiInfo
*      => c.SwaggerEndpoint("/swagger/V1/swagger.json", "ProductCatalog API");
*      => V1 must be matched
*  3. GlobalExceptionMiddleware
*      https://code-maze.com/global-error-handling-aspnetcore/
*  4. Using System.Text.Json in .NET Core 3.0
*      - https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-2/
*      - https://weblogs.thinktecture.com/pawel/2019/10/aspnet-core-3-0-custom-jsonconverter-for-the-new-system_text_json.html
*/
