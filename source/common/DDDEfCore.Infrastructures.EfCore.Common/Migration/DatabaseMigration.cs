using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DDDEfCore.Infrastructures.EfCore.Common.Migration
{
    public abstract class DatabaseMigration
    {
        protected DbContext DbContext { get; }

        protected DatabaseMigration(DbContext dbContext)
            => this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        protected abstract bool HasPendingMigrations();

        protected virtual async Task DoMigration()
        {
            await Task.FromResult(true);
        }

        public async Task ApplyMigration()
        {
            if (this.HasPendingMigrations())
            {
                await this.DoMigration();
            }
        }
    }
}
