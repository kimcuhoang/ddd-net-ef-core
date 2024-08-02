using MediatR;

namespace DDDEF.Application.UseCases.Projects.Commands.Create;

public class CreateProjectCommand(CreateProjectPayload payload) : IRequest
{
    public CreateProjectPayload Payload = payload;
}
