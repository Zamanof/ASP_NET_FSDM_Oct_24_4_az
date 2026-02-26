using MediatR;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Commands;

public record DeleteProjectCommand(int id):IRequest<bool>;
