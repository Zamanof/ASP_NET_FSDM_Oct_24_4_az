namespace ASP_NET_18._TaskFlow_Refactoring.DTOs;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}

public class CreateProjectRequest
{
    /// <summary>
    /// Project Name
    /// </summary>
    /// <example>My new project</example>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Project Description
    /// </summary>
    /// <example>Description for my project</example>
    public string Description { get; set; } = string.Empty;
}

public class UpdateProjectRequest
{
    /// <summary>
    /// The name of the project.
    /// </summary>
    /// <remarks>
    /// Should be a meaningful and human-readable project name.
    /// </remarks>
    /// <example>TaskFlow API Development</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A brief description of the project.
    /// </summary>
    /// <remarks>
    /// Can include additional details or notes about the project.
    /// </remarks>
    /// <example>API for managing projects and task items with full Swagger documentation</example>
    public string? Description { get; set; } = string.Empty;
}

public class ProjectMemberResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset JoinedAt { get; set; }
}

public class AvailableUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class AddProjectMemberRequest
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
}
