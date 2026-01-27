using ASP_NET_09._TaskFlow_Swagger_Documentation.Data;
using ASP_NET_09._TaskFlow_Swagger_Documentation.DTOs.Task_Items_DTOs;
using ASP_NET_09._TaskFlow_Swagger_Documentation.Models;
using ASP_NET_09._TaskFlow_Swagger_Documentation.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_09._TaskFlow_Swagger_Documentation.Services;

public class TaskItemService : ITaskItemService
{
    private readonly TaskFlowDbContext _context;
    private readonly IMapper _mapper;

    public TaskItemService(TaskFlowDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TaskItemResponseDto> CreateAsync(CreateTaskItemRequest createTaskItemRequest)
    {
        var isProjectExists = await _context
                                        .Projects
                                        .AnyAsync(p=> p.Id == createTaskItemRequest.ProjectId);

        if (!isProjectExists)
            throw new ArgumentException($"Project with ID {createTaskItemRequest.ProjectId} not found");
        var taskItem = _mapper.Map<TaskItem>(createTaskItemRequest);

       

        _context.TaskItems.Add(taskItem);
        await _context.SaveChangesAsync();

        await _context
                    .Entry(taskItem)
                    .Reference(t => t.Project)
                    .LoadAsync();

        return _mapper.Map<TaskItemResponseDto>(taskItem);
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

        return _mapper.Map<IEnumerable<TaskItemResponseDto>>(taskItems);
    }

    public async Task<TaskItemResponseDto?> GetByIdAsync(int id)
    {
        var taskItem = await _context
            .TaskItems
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t=> t.Id == id);

        if (taskItem is null) return null;

        return _mapper.Map<TaskItemResponseDto>(taskItem);
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId)
    {
        var taskItems = await _context
            .TaskItems
            .Include(t=>t.Project)
            .Where(t=> t.ProjectId == projectId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TaskItemResponseDto>>(taskItems);
    }

    public async Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemRequest updateTaskItemRequest)
    {
        var task = await _context
                                .TaskItems
                                .Include(t=>t.Project)
                                .FirstOrDefaultAsync(t=> t.Id == id);

        if (task is null) return null;

        _mapper.Map(updateTaskItemRequest, task);

        await _context.SaveChangesAsync();

        return _mapper.Map<TaskItemResponseDto>(task);
    }
}
