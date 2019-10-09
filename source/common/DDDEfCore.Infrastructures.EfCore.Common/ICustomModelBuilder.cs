using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.Infrastructures.EfCore.Common
{
    public interface ICustomModelBuilder
    {
        void Build(ModelBuilder modelBuilder);
    }
}
