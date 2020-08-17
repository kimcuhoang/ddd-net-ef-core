using AutoFixture;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.ProductCatalog.WebApi;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Queries.Tests
{
    public class SharedFixture : IAsyncLifetime
    {
        protected const string TestEnvironment = "Test";
        private readonly Checkpoint _checkpoint;

        public IHost Host { get; private set; }
        public IFixture Fixture { get; }

        public SharedFixture()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", TestEnvironment);

            this._checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            };
            this.Fixture = new Fixture();
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
            if (entities != null && entities.Any())
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

        private async Task ExecuteTest(Func<IServiceProvider, Task> testFunc)
        {
            var serviceScopeFactory = this.Host.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = serviceScopeFactory.CreateScope();
            await testFunc(scope.ServiceProvider);
        }

        public async Task ExecuteTestRequestHandler<TRequest, TResult>(TRequest request, Action<TResult> assert)
            where TRequest : IRequest<TResult>
        {
            var cancellationToken = new CancellationToken(false);
            var serviceScopeFactory = this.Host.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = serviceScopeFactory.CreateScope();

            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>();

            var result = await handler.Handle(request, cancellationToken);

            assert(result);
        }

        public Task ExecuteValidationTest<TRequest>(TRequest request, Action<TestValidationResult<TRequest, TRequest>> assert) where TRequest : class
        {
            var serviceScopeFactory = this.Host.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = serviceScopeFactory.CreateScope();
            var validator = scope.ServiceProvider.GetRequiredService<IValidator<TRequest>>();

            var result = validator.TestValidate(request);

            assert(result);

            return Task.CompletedTask;
        }
    }
}
