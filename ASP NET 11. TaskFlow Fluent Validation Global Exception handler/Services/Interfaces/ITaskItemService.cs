using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Common;
using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.DTOs.Task_Items_DTOs;

namespace ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Services.Interfaces;

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
