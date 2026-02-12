using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Task_Items_DTOs;

public class CreateTaskItemRequest
{
    /// <summary>
    /// Task Item Title
    /// </summary>
    /// <example>Do something</example>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Task Item Description
    /// </summary>
    /// <example>Description for task</example>
    public string Description { get; set; } = string.Empty;

    public TaskPriority Priority { get; set; }
    /// <summary>
    /// Project ID
    /// </summary>
    /// <example>1</example>
    public int ProjectId { get; set; }
}
