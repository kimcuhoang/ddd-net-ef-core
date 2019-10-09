using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DDDEfCore.Infrastructures.EfCore.Common.Migration
{
    public interface ISeedData<in TDbContext> where TDbContext : DbContext
    {
        Task SeedAsync(TDbContext context);
    }
}
