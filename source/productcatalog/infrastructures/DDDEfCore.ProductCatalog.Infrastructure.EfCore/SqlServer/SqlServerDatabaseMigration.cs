using DDDEfCore.Infrastructures.EfCore.Common.Migration;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.SqlServer
{
    public sealed class SqlServerDatabaseMigration : DatabaseMigration
    {
        public SqlServerDatabaseMigration(DbContext dbContext) : base(dbContext)
        {
        }

        #region Overrides of DatabaseMigration

        protected override bool HasPendingMigrations()
        {
            var latestAppliedMigrationId = this.DbContext.Database.GetAppliedMigrations().LastOrDefault();
            var latestPendingMigrationId = this.DbContext.Database.GetMigrations().LastOrDefault();

            return string.IsNullOrWhiteSpace(latestAppliedMigrationId) ||
                   (!string.IsNullOrWhiteSpace(latestPendingMigrationId) &&
                    latestAppliedMigrationId != latestPendingMigrationId);
        }

        protected override async Task DoMigration()
        {
            await this.DbContext.Database.MigrateAsync();
        }

        #endregion

        
    }
}
