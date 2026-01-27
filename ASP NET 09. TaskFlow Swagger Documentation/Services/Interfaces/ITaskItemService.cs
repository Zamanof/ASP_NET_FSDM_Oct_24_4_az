using ASP_NET_09._TaskFlow_Swagger_Documentation.DTOs.Task_Items_DTOs;
using ASP_NET_09._TaskFlow_Swagger_Documentation.Models;

namespace ASP_NET_09._TaskFlow_Swagger_Documentation.Services.Interfaces;

public interface ITaskItemService
{
    Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
    Task<TaskItemResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
    Task<TaskItemResponseDto> CreateAsync(CreateTaskItemRequest createTaskItemRequest);
    Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemRequest updateTaskItemRequest);
    Task<bool> DeleteAsync(int id);
}
