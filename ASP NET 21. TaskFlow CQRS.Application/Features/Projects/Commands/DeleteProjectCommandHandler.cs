using ASP_NET_21._TaskFlow_CQRS.Application.Repositories;
using AutoMapper;
using MediatR;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Commands;

class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly IProjectRepository _projectRepository;


    public DeleteProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.FindAsync(request.id);
        if (project == null) return false;
        await _projectRepository.RemoveAsync(project);
        return true;
    }
}
