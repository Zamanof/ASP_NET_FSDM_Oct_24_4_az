using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Common;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Project_DTOs;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Services;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy ="UserOrAbove")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    private IList<string> UserRoles 
        => User.Claims
               .Where(c => c.Type == ClaimTypes.Role)
               .Select(c => c.Value)
               .ToList();

    public ProjectsController(
                IProjectService projectService, 
                IAuthorizationService authorizationService)
    {
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    /// <remarks>
    /// Returns the full list of projects available in the system.
    /// </remarks>
    /// <returns>A collection of projects wrapped in ApiResponse.</returns>
    /// <response code="200">Projects were successfully retrieved.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectResponseDto>>>> GetAll()
    {
        var projects = await _projectService.GetAllForUserAsync(UserId!, UserRoles);
        return Ok(ApiResponse<IEnumerable<ProjectResponseDto>>.SuccessResponse(projects, "Projects returned successfully"));
    }

    /// <summary>
    /// Retrieves a project by its identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <returns>The project details wrapped in ApiResponse.</returns>
    /// <response code="200">The project was found and returned.</response>
    /// <response code="404">A project with the specified identifier was not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> GetById(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null) 
            return NotFound($"Project with ID {id} not found");

        var authResult = await _authorizationService
                                    .AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        
        if (!authResult.Succeeded) 
            return Forbid();

        var projectResponse = await _projectService.GetByIdAsync(id);

        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(projectResponse, "Project returned successfully"));
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="createProjectRequest">The payload used to create a project.</param>
    /// <returns>The created project wrapped in ApiResponse.</returns>
    /// <response code="201">The project was successfully created.</response>
    /// <response code="400">The request body is invalid or failed validation.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status400BadRequest)]
    [Authorize (Policy ="AdminOrManager")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create([FromBody] CreateProjectRequest createProjectRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ProjectResponseDto>.ErrorResponse("Validation failed", ModelState));

        var createdProject = await _projectService.CreateAsync(createProjectRequest, UserId!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProject.Id },
            ApiResponse<ProjectResponseDto>.SuccessResponse(createdProject, "Project created successfully")
        );
    }

    /// <summary>
    /// Updates an existing project by its identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="updateProjectRequest">The payload used to update a project.</param>
    /// <returns>The updated project wrapped in ApiResponse.</returns>
    /// <response code="200">The project was successfully updated.</response>
    /// <response code="400">The request body is invalid or failed validation.</response>
    /// <response code="404">A project with the specified identifier was not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponseDto>), StatusCodes.Status404NotFound)]
    
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Update(
        int id,
        [FromBody] UpdateProjectRequest updateProjectRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ProjectResponseDto>.ErrorResponse("Validation failed", ModelState));

        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null)
            return NotFound($"Project with ID {id} not found");

        var authResult = await _authorizationService
                                    .AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        var updatedProject = await _projectService.UpdateAsync(id, updateProjectRequest);

        if (updatedProject is null)
            return NotFound(ApiResponse<ProjectResponseDto>.ErrorResponse($"Project with ID {id} not found"));

        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(updatedProject, "Project updated successfully"));
    }

    /// <summary>
    /// Deletes a project by its identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <returns>A result wrapped in ApiResponse.</returns>
    /// <response code="200">The project was successfully deleted.</response>
    /// <response code="404">A project with the specified identifier was not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null)
            return NotFound($"Project with ID {id} not found");

        var authResult = await _authorizationService
                                    .AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

         await _projectService.DeleteAsync(id);        

        return NoContent();
    }

    [HttpGet("{projectId}/members")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMemberResponseDto>>>> 
        GetMembers(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound($"Project with ID {projectId} not found");

        var authResult = await _authorizationService
                                    .AuthorizeAsync(User, project, "ProjectMemberOrHigher");

        if (!authResult.Succeeded)
            return Forbid();

        var members = await _projectService.GetMembersAsync(projectId);

        return Ok(ApiResponse<IEnumerable<ProjectMemberResponseDto>>
            .SuccessResponse(members, "Members retrive successfully"));
    }

    [HttpGet("{projectId}/available-users")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AvailableUserDto>>>>
        GetAvailableUsersToAdd(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound($"Project with ID {projectId} not found");

        var authResult = await _authorizationService
                                    .AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        var users = await _projectService.GetAvailableUsersToAddAsync(projectId);

        return Ok(ApiResponse<IEnumerable<AvailableUserDto>>
            .SuccessResponse(users, "Users available to add"));
    }

    [HttpPost("{projectId}/members")]
    public async Task<IActionResult> AddMember(
                                            int projectId, 
                                            [FromBody] AddProjectMemberRequest request)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound($"Project with ID {projectId} not found");

        var authResult = await _authorizationService
                                    .AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        var userIdOrEmail = request.UserId ?? request.Email?.Trim();

        if (string.IsNullOrEmpty(userIdOrEmail))
            return BadRequest("Incorrect User Id or Email");

        var isAdded = await _projectService.AddMemberAsync(projectId, userIdOrEmail);

        if (!isAdded)
            return BadRequest("User Not Found or already added");

        return NoContent();
    }

    [HttpDelete("{projectId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(
                                            int projectId,
                                            string userId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null)
            return NotFound($"Project with ID {projectId} not found");

        var authResult = await _authorizationService
                                    .AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (!authResult.Succeeded)
            return Forbid();

        var isRemoved = await _projectService.RemoveMemberAsync(projectId, userId);

        if(!isRemoved) 
            return NotFound();
        return NoContent();
    }

}
