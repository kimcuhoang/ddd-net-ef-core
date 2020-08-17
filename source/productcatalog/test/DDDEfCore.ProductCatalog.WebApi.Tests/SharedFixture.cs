using AutoFixture;
using DDDEfCore.Core.Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Respawn;
using System;
using System.Data;
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
        protected const string TestEnvironment = "Test";
        private readonly Checkpoint _checkpoint;
        protected readonly IFixture AutoFixture;

        public IHost Host { get; private set; }
        public JsonSerializerOptions JsonSerializerOptions { get; private set; }

        public SharedFixture()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", TestEnvironment);

            this._checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };
            this.AutoFixture = new Fixture();
        }

        #region Implementation of IAsyncLifetime

        public virtual async Task InitializeAsync()
        {
            var hostBuilder = Program.CreateHostBuilder(new string[0]).UseEnvironment(TestEnvironment)
                                    .ConfigureWebHostDefaults(webHost =>
                                    {
                                        webHost.UseTestServer();
                                    });

            this.Host = await hostBuilder.StartAsync();
            this.JsonSerializerOptions = this.Host.Services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
        }

        public virtual async Task DisposeAsync()
        {
            var serviceScopeFactory = this.Host.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = serviceScopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
            var dbConnection = dbContext.Database.GetDbConnection();
            await dbConnection.OpenAsync();

            await this._checkpoint.Reset(dbConnection);
            await this.Host.StopAsync();
        }

        #endregion

        public async Task SeedingData<TAggregateRoot, TIdentity>(params TAggregateRoot[] entities) where TAggregateRoot : AggregateRoot<TIdentity> where TIdentity : IdentityBase
        {
            if (entities != null && entities.ToArray().Any())
            {
                var serviceScopeFactory = this.Host.Services.GetRequiredService<IServiceScopeFactory>();
                using var scope = serviceScopeFactory.CreateScope();
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
            using var client = this.Host.GetTestClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            await doTestFnc(client, this.JsonSerializerOptions);
        }
    }
}
