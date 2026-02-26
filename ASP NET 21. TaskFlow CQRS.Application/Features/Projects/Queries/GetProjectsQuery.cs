using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using MediatR;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Queries;

public record GetProjectsQuery(string UserId, IList<string> UserRoles):IRequest<IEnumerable<ProjectResponseDto>>;
