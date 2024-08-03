namespace DDDEF.API.Extensions;

internal static class WebApplicationExtensions
{
    public static WebApplication Construct(this WebApplication webApplication)
    {
        webApplication.UseSwagger();
        webApplication.UseSwaggerUI();

        webApplication
            .MapGet("/", () => TypedResults.Redirect("/swagger", permanent: true))
            .ExcludeFromDescription();

        return webApplication;
    }
}
