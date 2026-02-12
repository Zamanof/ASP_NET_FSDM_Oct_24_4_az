namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public IEnumerable<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    public IEnumerable<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}
