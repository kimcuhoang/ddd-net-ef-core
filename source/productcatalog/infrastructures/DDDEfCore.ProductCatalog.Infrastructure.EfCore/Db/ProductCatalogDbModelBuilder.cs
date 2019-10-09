using System.Reflection;
using DDDEfCore.Infrastructures.EfCore.Common;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Db
{
    public sealed class ProductCatalogDbModelBuilder : ICustomModelBuilder
    {
        #region Implementation of ICustomModelBuilder

        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        #endregion
    }
}
