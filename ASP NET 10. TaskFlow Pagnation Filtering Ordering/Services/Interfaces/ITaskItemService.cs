using ASP_NET_10._TaskFlow_Pagnation_Filtering_Ordering.Common;
using ASP_NET_10._TaskFlow_Pagnation_Filtering_Ordering.DTOs.Task_Items_DTOs;

namespace ASP_NET_10._TaskFlow_Pagnation_Filtering_Ordering.Services.Interfaces;

public interface ITaskItemService
{
    Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
    Task<PagedResult<TaskItemResponseDto>> GetPagedAsync(TaskItemQueryParams queryParams);
    Task<TaskItemResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
    Task<TaskItemResponseDto> CreateAsync(CreateTaskItemRequest createTaskItemRequest);
    Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemRequest updateTaskItemRequest);
    Task<bool> DeleteAsync(int id);
}
