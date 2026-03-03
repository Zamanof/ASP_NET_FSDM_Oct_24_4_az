using ASP_NET_21._TaskFlow_CQRS.Application.Common;
using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ASP_NET_21._TaskFlow_CQRS_Integration_tests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions 
        = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidDto_Returns200()
    {
        var email = $"test_{Guid.NewGuid():N}@example.com";
        var body = new
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var response 
            = await _client.PostAsJsonAsync("/api/auth/register", body, JsonOptions);

        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = 
            await response
            .Content
            .ReadFromJsonAsync<ApiResponse<AuthResponseDto>>(JsonOptions);

        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200()
    {
        var body = new { email = "admin@taskflow.com", password = "Admin123!" };

        var response
            = await _client.PostAsJsonAsync("/api/auth/login", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result
            = await response
            .Content
            .ReadFromJsonAsync<ApiResponse<AuthResponseDto>>(JsonOptions);

        result.Should().NotBeNull();
        result.Data!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401()
    {
        var body = new { email = "admin@taskflow.com", password = "Admin123" };

        var response
            = await _client.PostAsJsonAsync("/api/auth/login", body, JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProjects_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/projects");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    //[Fact]
    //public async Task Manager_CreateProject_returns201()
    //{
    //    var token = await GetManagerTokenAsync();
    //}
}
