namespace DDDEF.Application.UseCases.Projects.Commands.Create;

public class CreateProjectPayload
{
    public string Name { get; set; } = default!;
    public DateOnly StartDate { get; set; } = default!;
    public DateOnly EndDate { get; set; } = default!;
}
