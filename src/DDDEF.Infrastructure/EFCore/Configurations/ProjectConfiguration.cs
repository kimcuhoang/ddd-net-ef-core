using DDDEF.Core.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDDEF.Infrastructure.EFCore.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(_ => _.Id);
        builder.Property(_ => _.Id).HasConversion(_ => _.Id, _ => new ProjectId(_));
        builder.Property(_ => _.Name).HasMaxLength(100);
        builder.Property(_ => _.StartDate);
        builder.Property(_ => _.EndDate);
    }
}
