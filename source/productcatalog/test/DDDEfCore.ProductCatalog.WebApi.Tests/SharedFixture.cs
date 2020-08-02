using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures.JsonConverters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests
{
    public class SharedFixture : IAsyncLifetime
    {
        private readonly Checkpoint _checkpoint;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        protected readonly IFixture AutoFixture;
        private readonly IHost _host;

        public SharedFixture()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configurationBuilder.AddJsonFile("appsettings.json");
                });

            this._host = hostBuilder.Start();
            this._serviceScopeFactory = this._host.Services.GetService<IServiceScopeFactory>();
            
            this._jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            this._jsonSerializerOptions.Converters.Add(new IdentityJsonConverterFactory());

            this._checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };
            this.AutoFixture = new Fixture();
        }

        #region Implementation of IAsyncLifetime

        public virtual async Task InitializeAsync()
        {
            await this.ResetCheckpoint();
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;

        #endregion

        public async Task SeedingData<TAggregateRoot, TIdentity>(params TAggregateRoot[] entities) where TAggregateRoot : AggregateRoot<TIdentity> where TIdentity : IdentityBase
        {
            if (entities != null && entities.ToArray().Any())
            {
                using var scope = this._serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetService<DbContext>();
                await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
                try
                {
                    await dbContext.Set<TAggregateRoot>().AddRangeAsync(entities);
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task DoTest(Func<HttpClient, JsonSerializerOptions, Task> doTestFnc)
        {
            using var client = this._host.GetTestClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            await doTestFnc(client, this._jsonSerializerOptions);
        }

        private async Task ResetCheckpoint()
        {
            using var serviceScope = this._serviceScopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<DbContext>();

            var dbConnection = dbContext.Database.GetDbConnection();
            await dbConnection.OpenAsync();
            await this._checkpoint.Reset(dbConnection);
        }
    }
}
