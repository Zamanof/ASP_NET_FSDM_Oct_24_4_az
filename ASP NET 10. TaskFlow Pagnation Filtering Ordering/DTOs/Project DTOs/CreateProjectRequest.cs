namespace ASP_NET_10._TaskFlow_Pagnation_Filtering_Ordering.DTOs.Project_DTOs;
/// <summary>
/// Create project DTO. Uses for POST Requests
/// </summary>
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
