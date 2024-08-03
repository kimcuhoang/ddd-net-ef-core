using DDDEF.Core.Projects;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DDDEF.Infrastructure.EFCore;

public class ProjectManagementContext(DbContextOptions<ProjectManagementContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => this.Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
