using DDDEfCore.Infrastructures.EfCore.Common.Migration;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.SqlServer
{
    public sealed class SqlServerDatabaseMigration : DatabaseMigration
    {
        #region Overrides of DatabaseMigration

        protected override bool HasPendingMigrations(DbContext dbContext)
        {
            var latestAppliedMigrationId = dbContext.Database.GetAppliedMigrations().LastOrDefault();
            var latestPendingMigrationId = dbContext.Database.GetMigrations().LastOrDefault();

            return string.IsNullOrWhiteSpace(latestAppliedMigrationId) ||
                   (!string.IsNullOrWhiteSpace(latestPendingMigrationId) &&
                    latestAppliedMigrationId != latestPendingMigrationId);
        }

        protected override async Task DoMigration(DbContext dbContext)
        {
            await dbContext.Database.MigrateAsync();
        }

        #endregion
    }
}
