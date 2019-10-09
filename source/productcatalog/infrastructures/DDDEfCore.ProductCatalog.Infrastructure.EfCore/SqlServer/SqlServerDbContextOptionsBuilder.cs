using System;
using System.Reflection;
using DDDEfCore.Infrastructures.EfCore.Common;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.SqlServer
{
    public sealed class SqlServerDbContextOptionsBuilder : IExtendDbContextOptionsBuilder
    {
        #region Implementation of IExtendDbContextOptionsBuilder

        public DbContextOptionsBuilder Extend(DbContextOptionsBuilder optionsBuilder, 
                                        IDbConnStringFactory connectionStringFactory,
                                        string assemblyName)
        {

            var migrationFromAssemblyName = !string.IsNullOrWhiteSpace(assemblyName)
                ? assemblyName
                : Assembly.GetExecutingAssembly().FullName;

            return optionsBuilder.UseSqlServer(
                connectionStringFactory.Create(),
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.MigrationsAssembly(migrationFromAssemblyName);
                    //sqlServerOptionsAction.EnableRetryOnFailure(
                    //    15,
                    //    TimeSpan.FromSeconds(30),
                    //    null);
                }
            );
        }

        #endregion
    }
}
