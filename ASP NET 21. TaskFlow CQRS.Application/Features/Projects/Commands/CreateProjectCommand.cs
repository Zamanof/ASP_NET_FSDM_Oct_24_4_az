using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using MediatR;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Commands;

public record CreateProjectCommand(CreateProjectRequest Request, string OwnerId)
    :IRequest<ProjectResponseDto>;
