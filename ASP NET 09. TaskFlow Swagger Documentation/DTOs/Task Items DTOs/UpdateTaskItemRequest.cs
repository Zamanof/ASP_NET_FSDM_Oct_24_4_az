namespace ASP_NET_09._TaskFlow_Swagger_Documentation.DTOs.Task_Items_DTOs;

/// <summary>
/// Represents the data required to update an existing task item.
/// </summary>
/// <remarks>
/// This DTO is used when updating a task item within a project.
/// Only the provided fields will be updated according to the API implementation.
/// </remarks>
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
    public Models.TaskStatus Status { get; set; }
}
