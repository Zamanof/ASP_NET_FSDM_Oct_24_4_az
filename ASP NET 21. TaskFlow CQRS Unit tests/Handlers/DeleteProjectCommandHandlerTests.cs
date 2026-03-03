using ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Commands;
using ASP_NET_21._TaskFlow_CQRS.Application.Repositories;
using ASP_NET_21._TaskFlow_CQRS.Domain;
using FluentAssertions;
using Moq;

namespace ASP_NET_21._TaskFlow_CQRS_Unit_tests.Handlers;

public class DeleteProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_ProjectExists_RemoveAndReturnsTrue()
    {
        var projectRepo = new Mock<IProjectRepository>();
        var project = new Project
        {
            Id = 1,
            Name = "Project1",
            OwnerId = "user-1",
            CreatedAt = DateTimeOffset.UtcNow
        };

        projectRepo.Setup(r => r.FindAsync(1)).ReturnsAsync(project);

        var handler = new DeleteProjectCommandHandler(projectRepo.Object);

        var command = new DeleteProjectCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        projectRepo.Verify(r => r.RemoveAsync(project), Times.Once);
    }


    [Fact]
    public async Task Handle_ProjectNotFound_ReturnsFalse()
    {
        var projectRepo = new Mock<IProjectRepository>();

        projectRepo.Setup(r => r.FindAsync(1)).ReturnsAsync((Project?)null);

        var handler = new DeleteProjectCommandHandler(projectRepo.Object);

        var command = new DeleteProjectCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        projectRepo.Verify(r => r.RemoveAsync(It.IsAny<Project>()), Times.Never);
    }

}
