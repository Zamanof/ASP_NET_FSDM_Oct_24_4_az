namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Project_DTOs;

public class ProjectMemberResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset JoinedAt { get; set; }
}
