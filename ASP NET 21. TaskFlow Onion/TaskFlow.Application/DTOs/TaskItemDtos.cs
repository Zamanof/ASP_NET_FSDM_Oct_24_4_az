using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.DTOs;

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
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public int ProjectId { get; set; }
}

public class UpdateTaskItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskFlow.Domain.Entities.TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}

public class TaskStatusUpdateRequest
{
    public TaskFlow.Domain.Entities.TaskStatus Status { get; set; } = TaskFlow.Domain.Entities.TaskStatus.ToDo;
}
