using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;
using TaskStatus = ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models.TaskStatus;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Task_Items_DTOs;

public class TaskStatusUpdateRequest
{
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
}
