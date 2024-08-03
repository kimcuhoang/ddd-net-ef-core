using DDDEF.Infrastructure;

namespace DDDEF.API.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static WebApplication BuildWebApplication(this WebApplicationBuilder builder)
    {
        builder.AddInfrastructure();


        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder.Build();
    }

}
