using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.Infrastructures.EfCore.Common
{
    public interface IExtendDbContextOptionsBuilder
    {
        DbContextOptionsBuilder Extend(DbContextOptionsBuilder optionsBuilder,
                                        IDbConnStringFactory connectionStringFactory, 
                                        string assemblyName);
    }
}
