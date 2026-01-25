using ASP_NET_08._TaskFlow_DTO.DTOs.Task_Items_DTOs;
using ASP_NET_08._TaskFlow_DTO.Models;
using ASP_NET_08._TaskFlow_DTO.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_08._TaskFlow_DTO.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;

    public TaskItemsController(ITaskItemService taskItemService)
    {
        _taskItemService = taskItemService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetAll()
    {
        var tasks = await _taskItemService.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItemResponseDto>> GetById(int id)
    {
        var task = await _taskItemService.GetByIdAsync(id);
        if (task is null)
            return NotFound($"Tas kItem with ID {id} not found");
        return Ok(task);
    }

    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<IEnumerable<TaskItemResponseDto>>> GetByProjectId(int projectId)
    {
        var tasks = await _taskItemService.GetByProjectIdAsync(projectId);
        return Ok(tasks);
    }
    [HttpPost]
    public async Task<ActionResult<TaskItemResponseDto>> Create([FromBody] CreateTaskItemRequest createTaskItemRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var task = await _taskItemService.CreateAsync(createTaskItemRequest);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (ArgumentException ex)
        {

            return BadRequest(ex.Message);
        }

    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskItemResponseDto>> Update(int id, [FromBody] UpdateTaskItemRequest updateTaskItemRequest)
    {
        if(!ModelState.IsValid) 
            return BadRequest(ModelState);
        var task = await _taskItemService.UpdateAsync(id, updateTaskItemRequest);

        if (task is null)
            return NotFound($"Task Item with ID {id} not found");

        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var isDeleted = await _taskItemService.DeleteAsync(id);

        if(!isDeleted) 
            return NotFound($"Task Item with ID {id} not found");

        return NoContent();
    }
}
