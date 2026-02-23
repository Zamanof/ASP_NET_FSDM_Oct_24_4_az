using ASP_NET_20._TaskFlow.BLL.Common;
using ASP_NET_20._TaskFlow.BLL.DTOs;
using ASP_NET_20._TaskFlow.DAL;
using ASP_NET_20._TaskFlow.Models;
using AutoMapper;

namespace ASP_NET_20._TaskFlow.BLL.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskRepo;
    private readonly IMapper _mapper;

    public TaskItemService(ITaskItemRepository taskRepo, IMapper mapper) { _taskRepo = taskRepo; _mapper = mapper; }

    public async Task<TaskItemResponseDto> CreateAsync(CreateTaskItemRequest request)
    {
        if (!await _taskRepo.ProjectExistsAsync(request.ProjectId))
            throw new ArgumentException($"Project with ID {request.ProjectId} not found");
        var taskItem = _mapper.Map<TaskItem>(request);
        var added = await _taskRepo.AddAsync(taskItem);
        return _mapper.Map<TaskItemResponseDto>(added);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _taskRepo.FindAsync(id);
        if (task == null) return false;
        await _taskRepo.RemoveAsync(task);
        return true;
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetAllAsync()
    {
        var tasks = await _taskRepo.GetAllWithProjectAsync();
        return _mapper.Map<IEnumerable<TaskItemResponseDto>>(tasks);
    }

    public async Task<TaskItemResponseDto?> GetByIdAsync(int id)
    {
        var task = await _taskRepo.GetByIdWithProjectAsync(id);
        return task == null ? null : _mapper.Map<TaskItemResponseDto>(task);
    }

    public async Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId)
    {
        var tasks = await _taskRepo.GetByProjectIdAsync(projectId);
        return _mapper.Map<IEnumerable<TaskItemResponseDto>>(tasks);
    }

    public async Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemRequest request)
    {
        var task = await _taskRepo.GetByIdWithProjectAsync(id);
        if (task == null) return null;
        _mapper.Map(request, task);
        await _taskRepo.UpdateAsync(task);
        return _mapper.Map<TaskItemResponseDto>(task);
    }

    public async Task<PagedResult<TaskItemResponseDto>> GetPagedAsync(TaskItemQueryParams queryParams)
    {
        queryParams.Validate();
        var (items, total) = await _taskRepo.GetPagedAsync(queryParams.ProjectId, queryParams.Status, queryParams.Priority, queryParams.Search, queryParams.Sort, queryParams.SortDirection, queryParams.Page, queryParams.PageSize);
        var dtos = _mapper.Map<IEnumerable<TaskItemResponseDto>>(items);
        return PagedResult<TaskItemResponseDto>.Create(dtos, queryParams.Page, queryParams.PageSize, total);
    }

    public async Task<TaskItemResponseDto?> UpdateStatusAsync(int id, TaskStatusUpdateRequest request)
    {
        var task = await _taskRepo.GetByIdWithProjectAsync(id);
        if (task == null) return null;
        task.Status = request.Status;
        task.UpdatedAt = DateTime.UtcNow;
        await _taskRepo.UpdateAsync(task);
        return _mapper.Map<TaskItemResponseDto>(task);
    }

    public async Task<TaskItem?> GetTaskEntityAsync(int id) => await _taskRepo.GetByIdWithProjectAsync(id);
}
