using Microsoft.EntityFrameworkCore;

namespace DDD.ProductCatalog.WebApi.Infrastructures.HostedServices;

/// <summary>
/// https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-3/
/// </summary>
public class DbMigratorHostedService(
        IServiceProvider serviceProvider,
        ILogger<DbMigratorHostedService> logger) : IHostedService
{
    private readonly ILogger<DbMigratorHostedService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    #region Implementation of IHostedService

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = this._serviceProvider.CreateScope();

        var applicationDbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        var database = applicationDbContext.Database;

        var pendingChanges = await database.GetPendingMigrationsAsync(cancellationToken);

        if (!pendingChanges.Any()) 
        {
            this._logger.LogWarning("There is no pending migrations. Database is up to date!!!");
            return; 
        }

        await applicationDbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;


    #endregion
}
