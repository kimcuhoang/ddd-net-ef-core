using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DDDEfCore.Infrastructures.EfCore.Common.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<TAggregate> FindOneWithIncludeAsync<TAggregate, TIdentity>(this IRepository<TAggregate, TIdentity> repository,
                                                                                            Expression<Func<TAggregate, bool>> predicate,
                                                                                            Func<IQueryable<TAggregate>, IIncludableQueryable<TAggregate, object>> include = null)
            where TAggregate : AggregateRoot<TIdentity>
            where TIdentity : IdentityBase
        {
            var queryable = repository.AsQueryable();

            if (include != null)
            {
                queryable = include.Invoke(queryable);
            }

            return await queryable.FirstOrDefaultAsync(predicate);
        }
    }
}
