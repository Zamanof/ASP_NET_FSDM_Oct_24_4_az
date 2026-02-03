using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Common;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.DTOs.Task_Items_DTOs;

namespace ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Services;

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
