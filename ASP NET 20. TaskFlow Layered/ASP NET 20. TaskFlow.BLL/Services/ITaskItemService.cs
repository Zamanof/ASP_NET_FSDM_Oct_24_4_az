using ASP_NET_20._TaskFlow.BLL.Common;
using ASP_NET_20._TaskFlow.BLL.DTOs;
using ASP_NET_20._TaskFlow.Models;

namespace ASP_NET_20._TaskFlow.BLL.Services;

public interface ITaskItemService
{
    Task<TaskItemResponseDto> CreateAsync(CreateTaskItemRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
    Task<TaskItemResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
    Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemRequest request);
    Task<PagedResult<TaskItemResponseDto>> GetPagedAsync(TaskItemQueryParams queryParams);
    Task<TaskItemResponseDto?> UpdateStatusAsync(int id, TaskStatusUpdateRequest request);
    Task<TaskItem?> GetTaskEntityAsync(int id);
}
