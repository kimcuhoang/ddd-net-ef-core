using DDDEF.Core;
using MediatR;

namespace DDDEF.Application.UseCases.Projects.Commands.Create;

public class CreateProjectHandler(IRepositoryFactory repositoryFactory) : IRequestHandler<CreateProjectCommand>
{
    private readonly IRepositoryFactory _repositoryFactory = repositoryFactory;
    
    public Task Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
