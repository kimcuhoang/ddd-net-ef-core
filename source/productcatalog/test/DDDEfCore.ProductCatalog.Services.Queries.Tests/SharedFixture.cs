using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.WebApi;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests
{
    public class SharedFixture : IAsyncLifetime
    {
        private readonly Checkpoint _checkpoint;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly IFixture Fixture;
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

            this._checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };

            this.Fixture = new Fixture();
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

        private async Task ExecuteTest(Func<IServiceProvider, Task> testFunc)
        {
            using var scope = this._serviceScopeFactory.CreateScope();
            await testFunc(scope.ServiceProvider);
        }

        public async Task ExecuteTestRequestHandler<TRequest, TResult>(TRequest request, Action<TResult> assert)
            where TRequest : IRequest<TResult>
        {
            var cancellationToken = new CancellationToken(false);
            using var scope = this._serviceScopeFactory.CreateScope();

            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>();

            var result = await handler.Handle(request, cancellationToken);

            assert(result);
        }

        public Task ExecuteValidationTest<TRequest>(TRequest request, Action<TestValidationResult<TRequest, TRequest>> assert) where TRequest : class
        {
            using var scope = this._serviceScopeFactory.CreateScope();
            var validator = scope.ServiceProvider.GetRequiredService<IValidator<TRequest>>();

            var result = validator.TestValidate(request);

            assert(result);

            return Task.CompletedTask;
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
