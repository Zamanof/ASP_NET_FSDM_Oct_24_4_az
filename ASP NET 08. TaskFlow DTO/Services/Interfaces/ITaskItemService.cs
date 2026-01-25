using ASP_NET_08._TaskFlow_DTO.DTOs.Task_Items_DTOs;
using ASP_NET_08._TaskFlow_DTO.Models;

namespace ASP_NET_08._TaskFlow_DTO.Services.Interfaces;

public interface ITaskItemService
{
    Task<IEnumerable<TaskItemResponseDto>> GetAllAsync();
    Task<TaskItemResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItemResponseDto>> GetByProjectIdAsync(int projectId);
    Task<TaskItemResponseDto> CreateAsync(CreateTaskItemRequest createTaskItemRequest);
    Task<TaskItemResponseDto?> UpdateAsync(int id, UpdateTaskItemRequest updateTaskItemRequest);
    Task<bool> DeleteAsync(int id);
}
