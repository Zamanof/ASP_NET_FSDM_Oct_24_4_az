using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASP_NET_21._TaskFlow_CQRS.Application.Common;
using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_21._TaskFlow_CQRS.Application.Services;
using MediatR;
using ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Queries;
using ASP_NET_21._TaskFlow_CQRS.Application.Features.Projects.Commands;

namespace ASP_NET_21._TaskFlow_CQRS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMediator _mediator;

    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    private IList<string> UserRoles => User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

    public ProjectsController(IProjectService projectService, IAuthorizationService authorizationService, IMediator mediator)
    {
        _projectService = projectService;
        _authorizationService = authorizationService;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectResponseDto>>>> GetAll()
    {
        var projects = await _mediator.Send(new GetProjectsQuery(UserId!, UserRoles));
        return Ok(ApiResponse<IEnumerable<ProjectResponseDto>>.SuccessResponse(projects, "Projects returned successfully"));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> GetById(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);
        if (project is null) return NotFound($"Project with ID {id} not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authResult.Succeeded) return Forbid();
        var projectResponse = await _mediator.Send(new GetProjectByIdQuery(id));
        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(projectResponse!, "Project returned successfully"));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create([FromBody] CreateProjectRequest createProjectRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse<ProjectResponseDto>.ErrorResponse("Validation failed", ModelState));
        var createdProject = await _mediator.Send(new CreateProjectCommand(createProjectRequest, UserId!));
        return CreatedAtAction(nameof(GetById), new { id = createdProject.Id }, ApiResponse<ProjectResponseDto>.SuccessResponse(createdProject, "Project created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Update(int id, [FromBody] UpdateProjectRequest updateProjectRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse<ProjectResponseDto>.ErrorResponse("Validation failed", ModelState));
        var project = await _projectService.GetProjectEntityAsync(id);
        if (project is null) return NotFound($"Project with ID {id} not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();
        var updatedProject = await _mediator.Send(new UpdateProjectCommand(id, updateProjectRequest));
        if (updatedProject is null) return NotFound(ApiResponse<ProjectResponseDto>.ErrorResponse($"Project with ID {id} not found"));
        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(updatedProject, "Project updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);
        if (project is null) return NotFound($"Project with ID {id} not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();
        await _mediator.Send(new DeleteProjectCommand(id));
        return NoContent();
    }

    [HttpGet("{projectId}/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMemberResponseDto>>>> GetMembers(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project is null) return NotFound($"Project with ID {projectId} not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (!authResult.Succeeded) return Forbid();
        var members = await _projectService.GetMembersAsync(projectId);
        return Ok(ApiResponse<IEnumerable<ProjectMemberResponseDto>>.SuccessResponse(members, "Members retrieved successfully"));
    }

    [HttpGet("{projectId}/available-users")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AvailableUserDto>>>> GetAvailableUsersToAdd(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project is null) return NotFound($"Project with ID {projectId} not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();
        var users = await _projectService.GetAvailableUsersToAddAsync(projectId);
        return Ok(ApiResponse<IEnumerable<AvailableUserDto>>.SuccessResponse(users, "Users available to add"));
    }

    [HttpPost("{projectId}/members")]
    public async Task<IActionResult> AddMember(int projectId, [FromBody] AddProjectMemberRequest request)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project is null) return NotFound($"Project with ID {projectId} not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();
        var userIdOrEmail = request.UserId ?? request.Email?.Trim();
        if (string.IsNullOrEmpty(userIdOrEmail)) return BadRequest("Incorrect User Id or Email");
        var isAdded = await _projectService.AddMemberAsync(projectId, userIdOrEmail);
        if (!isAdded) return BadRequest("User not found or already added");
        return NoContent();
    }

    [HttpDelete("{projectId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(int projectId, string userId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project is null) return NotFound($"Project with ID {projectId} not found");
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (!authResult.Succeeded) return Forbid();
        var isRemoved = await _projectService.RemoveMemberAsync(projectId, userId);
        if (!isRemoved) return NotFound();
        return NoContent();
    }
}
