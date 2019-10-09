using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DDDEfCore.Infrastructures.EfCore.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.Infrastructures.EfCore.Common.Migration
{
    public abstract class DatabaseMigration
    {
        protected abstract bool HasPendingMigrations(DbContext dbContext);

        protected virtual async Task DoMigration(DbContext dbContext)
        {
            await Task.FromResult(true);
        }

        public async Task ApplyMigration(DbContext dbContext)
        {
            if (this.HasPendingMigrations(dbContext))
            {
                await this.DoMigration(dbContext);
            }
        }
    }
}
