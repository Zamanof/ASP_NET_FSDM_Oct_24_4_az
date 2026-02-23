using ASP_NET_20._TaskFlow.BLL.Common;
using ASP_NET_20._TaskFlow.BLL.DTOs;
using ASP_NET_20._TaskFlow.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASP_NET_20._TaskFlow.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "UserOrAbove")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    public TaskItemsController(ITaskItemService taskItemService, IAuthorizationService authorizationService, IProjectService projectService)
    {
        _taskItemService = taskItemService;
        _authorizationService = authorizationService;
        _projectService = projectService;
    }

    private static Dictionary<string, string[]> ToValidationErrors(ModelStateDictionary modelState) =>
        modelState.Where(x => x.Value?.Errors.Count > 0).ToDictionary(k => k.Key, v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetAll()
    {
        var tasks = await _taskItemService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(tasks, "Task items returned successfully"));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TaskItemResponseDto>>>> GetPaged([FromQuery] TaskItemQueryParams queryParams)
    {
        var tasks = await _taskItemService.GetPagedAsync(queryParams);
        return Ok(ApiResponse<PagedResult<TaskItemResponseDto>>.SuccessResponse(tasks, "Task items returned successfully"));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> GetById(int id)
    {
        var taskEntity = await _taskItemService.GetTaskEntityAsync(id);
        if (taskEntity is null) return NotFound();
        var project = await _projectService.GetProjectEntityAsync(taskEntity.ProjectId);
        if (project is null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (authResult is null || !authResult.Succeeded) return Forbid();
        var task = await _taskItemService.GetByIdAsync(id);
        if (task is null) return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task item with ID {id} not found"));
        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item returned successfully"));
    }

    [HttpGet("project/{projectId:int}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemResponseDto>>>> GetByProjectId(int projectId)
    {
        var project = await _projectService.GetProjectEntityAsync(projectId);
        if (project is null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectMemberOrHigher");
        if (authResult is null || !authResult.Succeeded) return Forbid();
        var tasks = await _taskItemService.GetByProjectIdAsync(projectId);
        if (tasks is null) return NotFound(ApiResponse<IEnumerable<TaskItemResponseDto>>.ErrorResponse($"Project with ID {projectId} not found"));
        return Ok(ApiResponse<IEnumerable<TaskItemResponseDto>>.SuccessResponse(tasks, "Task items returned successfully"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Create([FromBody] CreateTaskItemRequest createTaskItemRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse("Validation failed", ToValidationErrors(ModelState)));
        var project = await _projectService.GetProjectEntityAsync(createTaskItemRequest.ProjectId);
        if (project is null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (authResult is null || !authResult.Succeeded) return Forbid();
        var task = await _taskItemService.CreateAsync(createTaskItemRequest);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> Update(int id, [FromBody] UpdateTaskItemRequest updateTaskItemRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse("Validation failed", ToValidationErrors(ModelState)));
        var taskEntity = await _taskItemService.GetTaskEntityAsync(id);
        if (taskEntity is null) return NotFound();
        var project = await _projectService.GetProjectEntityAsync(taskEntity.ProjectId);
        if (project is null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (authResult is null || !authResult.Succeeded) return Forbid();
        var task = await _taskItemService.UpdateAsync(id, updateTaskItemRequest);
        if (task is null) return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task item with ID {id} not found"));
        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item updated successfully"));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<TaskItemResponseDto>>> TaskStatusUpdate(int id, [FromBody] TaskStatusUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ApiResponse<TaskItemResponseDto>.ErrorResponse("Validation failed", ToValidationErrors(ModelState)));
        var taskEntity = await _taskItemService.GetTaskEntityAsync(id);
        if (taskEntity is null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, taskEntity, "TaskStatusChange");
        if (authResult is null || !authResult.Succeeded) return Forbid();
        var task = await _taskItemService.UpdateStatusAsync(id, request);
        if (task is null) return NotFound(ApiResponse<TaskItemResponseDto>.ErrorResponse($"Task item with ID {id} not found"));
        return Ok(ApiResponse<TaskItemResponseDto>.SuccessResponse(task, "Task item updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var taskEntity = await _taskItemService.GetTaskEntityAsync(id);
        if (taskEntity is null) return NotFound();
        var project = await _projectService.GetProjectEntityAsync(taskEntity.ProjectId);
        if (project is null) return NotFound();
        var authResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectOwnerOrAdmin");
        if (authResult is null || !authResult.Succeeded) return Forbid();
        var isDeleted = await _taskItemService.DeleteAsync(id);
        if (!isDeleted) return NotFound(ApiResponse<object?>.ErrorResponse($"Task item with ID {id} not found"));
        return NoContent();
    }
}
