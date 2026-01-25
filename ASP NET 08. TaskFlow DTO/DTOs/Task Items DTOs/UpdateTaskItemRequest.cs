using ASP_NET_08._TaskFlow_DTO.Models;

namespace ASP_NET_08._TaskFlow_DTO.DTOs.Task_Items_DTOs;

public class UpdateTaskItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; }
}
