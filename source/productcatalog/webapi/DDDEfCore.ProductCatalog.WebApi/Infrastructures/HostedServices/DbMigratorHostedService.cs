using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.WebApi.Infrastructures.HostedServices
{
    /// <summary>
    /// https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-3/
    /// </summary>
    public class DbMigratorHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public DbMigratorHostedService(IServiceProvider serviceProvider)
            => this._serviceProvider = serviceProvider;

        #region Implementation of IHostedService

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = this._serviceProvider.CreateScope();

            var applicationDbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

            await applicationDbContext.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;


        #endregion
    }
}
