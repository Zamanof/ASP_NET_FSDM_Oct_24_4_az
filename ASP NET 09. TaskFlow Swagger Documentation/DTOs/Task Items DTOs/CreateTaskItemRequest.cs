namespace ASP_NET_09._TaskFlow_Swagger_Documentation.DTOs.Task_Items_DTOs;

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
    /// <summary>
    /// Project ID
    /// </summary>
    /// <example>1</example>
    public int ProjectId { get; set; }
}
