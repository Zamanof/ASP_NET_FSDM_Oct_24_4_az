namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Project_DTOs;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}
