using ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Queries;
using ASP_NET_21._TaskFlow_CQRS.Application.Mappings;
using ASP_NET_21._TaskFlow_CQRS.Application.Repositories;
using ASP_NET_21._TaskFlow_CQRS.Domain;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace ASP_NET_21._TaskFlow_CQRS_Unit_tests.Handlers;

public class GetProjectsQueryHandlerTests
{
    private static readonly IMapper Mapper =
        new MapperConfiguration(config => config.AddProfile<MappingProfile>())
        .CreateMapper();

    [Fact]
    public async Task Handle_CallsGetAllForUserAsyncAndReturnsMappedDtos()
    {
        // Arrange
        var projectRepo = new Mock<IProjectRepository>();

        var projects = new List<Project>
        {
            new Project()
            {
                Id=1,
                Name="Pr1",
                OwnerId = "u1",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Project()
            {
                Id=2,
                Name="Pr1",
                OwnerId = "u2",
                CreatedAt = DateTimeOffset.UtcNow
            },
             new Project()
            {
                Id=3,
                Name="Pr1",
                OwnerId = "u1",
                CreatedAt = DateTimeOffset.UtcNow
            },
        };

        projectRepo.Setup(r => r.GetAllForUserAsync("u1", It.IsAny<IList<string>>()))
            .ReturnsAsync(projects);

        var handler = new GetProjectsQueryHandler(projectRepo.Object, Mapper);

        var query = new GetProjectsQuery("u1", new List<string> { "User" });

        // Act

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Pr1");
        projectRepo.Verify(r => r.GetAllForUserAsync("u1", It.IsAny<IList<string>>())
        ,Times.Once);
    }
}
