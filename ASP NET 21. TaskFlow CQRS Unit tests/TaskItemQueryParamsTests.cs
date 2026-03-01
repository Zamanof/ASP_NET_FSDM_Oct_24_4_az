using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using FluentAssertions;

namespace ASP_NET_21._TaskFlow_CQRS_Unit_tests;

public class TaskItemQueryParamsTests
{
    [Theory]
    [InlineData(0, 10, 1, 10)]
    [InlineData(1, 0, 1, 1)]
    [InlineData(2, 200, 2, 100)]
    [InlineData(3, 20, 3, 20)]
    public void Validate_NormalizesPageAndSize(
        int page, 
        int size, 
        int expectedPage,
        int expectedSize)
    {

        var param = new TaskItemQueryParams { Page = page, PageSize = size };

        param.Validate();

        param.Page.Should().Be(expectedPage);
        param.PageSize.Should().Be(expectedSize);

    }
}
