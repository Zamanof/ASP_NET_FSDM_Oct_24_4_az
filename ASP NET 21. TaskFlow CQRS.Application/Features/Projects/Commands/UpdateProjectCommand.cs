using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using MediatR;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Commands;

public record UpdateProjectCommand(int id, UpdateProjectRequest request): IRequest<ProjectResponseDto?>;
