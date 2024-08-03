using Microsoft.AspNetCore.Builder;

namespace DDDEF.Infrastructure;
public static class InfrastructureRegistration
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder, string nameOfConnectionString = "Default")
    {
        builder.AddEFCore(nameOfConnectionString);
        return builder;
    }
}
