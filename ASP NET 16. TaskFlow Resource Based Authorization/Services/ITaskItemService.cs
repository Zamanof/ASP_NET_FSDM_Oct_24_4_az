using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Common;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Task_Items_DTOs;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Services;

public interface ITaskItemService
{
    Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
    Task<PagedResult<TaskItemResponseDto>> GetPagedAsync(TaskItemQueryParams queryParams);
    Task<TaskItemResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
    Task<TaskItemResponseDto> CreateAsync(CreateTaskItemRequest createTaskItemRequest);
    Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemRequest updateTaskItemRequest);
    Task<TaskItemResponseDto?> UpdateStatusAsync(int id, TaskStatusUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}
