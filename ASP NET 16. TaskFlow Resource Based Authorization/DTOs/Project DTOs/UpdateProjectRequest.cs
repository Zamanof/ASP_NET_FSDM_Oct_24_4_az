namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Project_DTOs;

/// <summary>
/// Represents the data required to update an existing project.
/// </summary>
/// <remarks>
/// This DTO is used when updating a project.  
/// Only the provided fields will be updated based on the API implementation.
/// </remarks>
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
