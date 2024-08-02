using DDDEF.Core.Abstractions;

namespace DDDEF.Core.Projects;

public record ProjectId : IdBase
{
    public ProjectId(Guid Id) : base(Id)
    {
    }
}

public class Project(ProjectId id) : AggregationRoot<ProjectId>(id)
{
    public string Name { get; set; } = default!;
    public DateOnly StartDate { get; set; } = default!;
    public DateOnly EndDate {get; set; } = default!;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Id;
    }
}
