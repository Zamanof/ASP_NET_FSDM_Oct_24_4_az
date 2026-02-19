using ASP_NET_19._TaskFlow_Files.Models;
using TaskStatus = ASP_NET_19._TaskFlow_Files.Models.TaskStatus;

namespace ASP_NET_19._TaskFlow_Files.DTOs;

public class TaskItemResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}

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

public class UpdateTaskItemRequest
{
    /// <summary>
    /// The title of the task item.
    /// </summary>
    /// <remarks>
    /// Should be a short and clear description of the task.
    /// </remarks>
    /// <example>Implement Swagger documentation</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the task item.
    /// </summary>
    /// <remarks>
    /// Can contain additional information, requirements, or notes related to the task.
    /// </remarks>
    /// <example>Add XML comments and response descriptions for all endpoints</example>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The current status of the task item.
    /// </summary>
    /// <remarks>
    /// Represents the workflow state of the task (e.g. Pending, InProgress, Completed).
    /// </remarks>
    /// <example>InProgress</example>
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}

public class TaskStatusUpdateRequest
{
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
}


