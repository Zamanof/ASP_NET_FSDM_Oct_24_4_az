using ASP_NET_08._TaskFlow_DTO.Data;
using ASP_NET_08._TaskFlow_DTO.DTOs.Task_Items_DTOs;
using ASP_NET_08._TaskFlow_DTO.Models;
using ASP_NET_08._TaskFlow_DTO.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_08._TaskFlow_DTO.Services;

public class TaskItemService : ITaskItemService
{
    private readonly TaskFlowDbContext _context;

    public TaskItemService(TaskFlowDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItemResponseDto> CreateAsync(CreateTaskItemRequest createTaskItemRequest)
    {
        var isProjectExists = await _context
                                        .Projects
                                        .AnyAsync(p=> p.Id == createTaskItemRequest.ProjectId);

        if (!isProjectExists)
            throw new ArgumentException($"Project with ID {createTaskItemRequest.ProjectId} not found");
        var taskItem = new TaskItem
        {
            Title = createTaskItemRequest.Title,
            Description = createTaskItemRequest.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            Status = Models.TaskStatus.ToDo,
            ProjectId = createTaskItemRequest.ProjectId
        };

       

        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();

        await _context
                    .Entry(taskItem)
                    .Reference(t => t.Project)
                    .LoadAsync();

        return MapToRespnseDto(taskItem);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.TaskItems.FindAsync(id);

        if (task is null) return false;

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetAllAsync()
    {
        var taskItems = await _context
                        .TaskItems
                        .Include(t => t.Project)
                        .ToListAsync();
        return taskItems.Select(MapToRespnseDto);
    }

    public async Task<TaskItemResponseDto?> GetByIdAsync(int id)
    {
        var taskItem = await _context
            .TaskItems
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t=> t.Id == id);
        if (taskItem is null) return null;
        return MapToRespnseDto(taskItem!);
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId)
    {
        var taskItems = await _context
            .TaskItems
            .Include(t=>t.Project)
            .Where(t=> t.ProjectId == projectId)
            .ToListAsync();
       return taskItems.Select(MapToRespnseDto);
    }

    public async Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemRequest updateTaskItemRequest)
    {
        var task = await _context
                                .TaskItems
                                .Include(t=>t.Project)
                                .FirstOrDefaultAsync(t=> t.Id == id);

        if (task is null) return null;

        task.Title = updateTaskItemRequest.Title;
        task.Description = updateTaskItemRequest.Description;
        task.Status = updateTaskItemRequest.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToRespnseDto(task);
    }

    private TaskItemResponseDto MapToRespnseDto(TaskItem taskItem)
    {
        return new TaskItemResponseDto
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status.ToString(),
            ProjectId = taskItem.ProjectId,
            ProjectName = taskItem.Project.Name
        };
    }
}
