using Microsoft.AspNetCore.Builder;

namespace DDDEF.Infrastructure;

internal static class EFCoreRegistration
{
    public static WebApplicationBuilder AddEFCore(this WebApplicationBuilder builder)
    {
        return builder;
    }
}
