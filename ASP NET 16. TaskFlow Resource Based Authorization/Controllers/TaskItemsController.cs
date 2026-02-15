using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Common;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Task_Items_DTOs;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy ="UserOrAbove")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    public TaskItemsController(
        ITaskItemService taskItemService, 
        IAuthorizationService authorizationService, 
        IProjectService projectService)
    {
        _taskItemService = taskItemService;
        _authorizationService = authorizationService;
        _projectService = projectService;
    }

    private static Dictionary<string, string[]> ToValidationErrors(ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                k => k.Key,
                v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );
    }

    /// <summary>
    /// Retrieves all task items.
    /// </summary>
    /// <remarks>
    /// Returns the full list of task items available in the system.
    /// </remarks>
    /// <returns>A collection of task items wrapped in ApiResponse.</returns>
    /// <response code="200">Task items were successfully retrieved.</response>
    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskItemResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetAll()
    {
        var tasks = await _taskItemService.GetAllAsync();

        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(tasks, "Task items returned successfully"));
    }

    /// <summary>
    /// Retrieves all task items. Filtered, Paginated
    /// </summary>
    /// <remarks>
    /// Returns the full list of task items available in the system.
    /// </remarks>
    /// <returns>A collection of task items wrapped in ApiResponse.</returns>
    /// <response code="200">Task items were successfully retrieved.</response>

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TaskItemResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<TaskItemResponseDto>>>> GetPaged([FromQuery]TaskItemQueryParams queryParams)
    {
        var tasks = await _taskItemService.GetPagedAsync(queryParams);

        return Ok(ApiResponse<PagedResult<TaskItemResponseDto>>.SuccessResponse(tasks, "Task items returned successfully"));
    }

    /// <summary>
    /// Retrieves a task item by its identifier.
    /// </summary>
    /// <param name="id">The task item identifier.</param>
    /// <returns>The task item details wrapped in ApiResponse.</returns>
    /// <response code="200">The task item was found and returned.</response>
    /// <response code="404">A task item with the specified identifier was not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> GetById(int id)
    {
        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null) return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHiger");

        if (authResult is null) return Forbid();

        //throw new NullReferenceException();
        var task = await _taskItemService.GetByIdAsync(id);

        if (task is null)
            return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task item with ID {id} not found"));

        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item returned successfully"));
    }

    /// <summary>
    /// Retrieves all task items for a specific project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <returns>A collection of task items belonging to the specified project wrapped in ApiResponse.</returns>
    /// <response code="200">Task items for the specified project were successfully retrieved.</response>
    /// <response code="404">Project with the specified identifier was not found (if enforced by the service).</response>
    [HttpGet("project/{projectId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskItemResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaskItemResponseDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetByProjectId(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);

        if (project is null) return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHiger");
        
        if (authResult is null) return Forbid();

        var tasks = await _taskItemService.GetByProjectIdAsync(projectId);

        if (tasks is null)
            return NotFound(ApiResponse<IEnumerable<TaskItemResponseDto>>.ErrorResponse($"Project with ID {projectId} not found"));

        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(tasks, "Task items returned successfully"));
    }

    /// <summary>
    /// Creates a new task item.
    /// </summary>
    /// <param name="createTaskItemRequest">The payload used to create a task item.</param>
    /// <returns>The created task item wrapped in ApiResponse.</returns>
    /// <response code="201">The task item was successfully created.</response>
    /// <response code="400">The request body is invalid, failed validation, or references invalid related data.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Create([FromBody] CreateTaskItemRequest createTaskItemRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse("Validation failed", ToValidationErrors(ModelState)));

        var project = await _projectService.GetProjectEntityAsync(createTaskItemRequest.ProjectId);

        if (project is null) return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (authResult is null) return Forbid();

        var task = await _taskItemService.CreateAsync(createTaskItemRequest);

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item created successfully")
        );
    }

    /// <summary>
    /// Updates an existing task item by its identifier.
    /// </summary>
    /// <param name="id">The task item identifier.</param>
    /// <param name="updateTaskItemRequest">The payload used to update a task item.</param>
    /// <returns>The updated task item wrapped in ApiResponse.</returns>
    /// <response code="200">The task item was successfully updated.</response>
    /// <response code="400">The request body is invalid, failed validation, or contains invalid changes.</response>
    /// <response code="404">A task item with the specified identifier was not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<TaskItemResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Update(int id, [FromBody] UpdateTaskItemRequest updateTaskItemRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse("Validation failed", ToValidationErrors(ModelState)));

        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null) return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (authResult is null) return Forbid();


        var task = await _taskItemService.UpdateAsync(id, updateTaskItemRequest);

        if (task is null)
            return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task item with ID {id} not found"));

        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item updated successfully"));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> TaskStatusUpdate(int id, [FromBody] TaskStatusUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse("Validation failed", ToValidationErrors(ModelState)));

        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null) return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "TaskStatusChange");

        if (authResult is null) return Forbid();


        var task = await _taskItemService.UpdateStatusAsync(id, request);

        if (task is null)
            return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task item with ID {id} not found"));

        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item updated successfully"));
    }

    /// <summary>
    /// Deletes a task item by its identifier.
    /// </summary>
    /// <param name="id">The task item identifier.</param>
    /// <returns>A result wrapped in ApiResponse.</returns>
    /// <response code="200">The task item was successfully deleted.</response>
    /// <response code="404">A task item with the specified identifier was not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse("Validation failed", ToValidationErrors(ModelState)));

        var project = await _projectService.GetProjectEntityAsync(id);

        if (project is null) return NotFound();

        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");

        if (authResult is null) return Forbid();
        var isDeleted = await _taskItemService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound(ApiResponse<object?>.ErrorResponse($"Task item with ID {id} not found"));

        return Ok(ApiResponse<object?>.SuccessResponse(null, "Task item deleted successfully"));
    }
}
