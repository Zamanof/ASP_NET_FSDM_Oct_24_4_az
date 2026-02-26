using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_21._TaskFlow_CQRS.Application.Repositories;
using ASP_NET_21._TaskFlow_CQRS.Domain;
using AutoMapper;
using MediatR;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Commands;

class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectResponseDto?>
{
    private readonly IProjectRepository _projectRepo;
    private readonly IMapper _mapper;

    public UpdateProjectCommandHandler(IProjectRepository projectRepo, IMapper mapper)
    {
        _projectRepo = projectRepo;
        _mapper = mapper;
    }

    public async Task<ProjectResponseDto?> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepo.GetByIdWithTasksAsync(request.id);
        if (project == null) return null;
        _mapper.Map(request, project);
        await _projectRepo.UpdateAsync(project);
        return _mapper.Map<ProjectResponseDto>(project);
    }
}
