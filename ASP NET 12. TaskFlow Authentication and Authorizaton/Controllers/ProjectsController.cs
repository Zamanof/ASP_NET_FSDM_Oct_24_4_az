using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Common;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.DTOs.Project_DTOs;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
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
        var projects = await _projectService.GetAllAsync();
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
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
            return NotFound(ApiResponse<ProjectResponseDto>.ErrorResponse($"Project with ID {id} not found"));

        return Ok(ApiResponse<ProjectResponseDto>.SuccessResponse(project, "Project returned successfully"));
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
    [Authorize]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create([FromBody] CreateProjectRequest createProjectRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ProjectResponseDto>.ErrorResponse("Validation failed", ModelState));

        var createdProject = await _projectService.CreateAsync(createProjectRequest);

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
    public async Task<ActionResult<ApiResponse<object?>>> Delete(int id)
    {
        var deleted = await _projectService.DeleteAsync(id);

        if (!deleted)
            return NotFound(ApiResponse<object?>.ErrorResponse($"Project with ID {id} not found"));

        // Wrapper-lə 204 qaytarsan body göstərilməyəcək. Ona görə vahid wrapper üçün 200 daha məntiqlidir.
        return Ok(ApiResponse<object?>.SuccessResponse(null, "Project deleted successfully"));
    }
}
