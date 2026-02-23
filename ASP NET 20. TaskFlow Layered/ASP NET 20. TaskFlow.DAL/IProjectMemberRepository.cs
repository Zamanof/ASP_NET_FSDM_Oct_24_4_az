using ASP_NET_20._TaskFlow.Models;

namespace ASP_NET_20._TaskFlow.DAL;

public interface IProjectMemberRepository
{
    Task<IEnumerable<ProjectMember>> GetByProjectIdWithUserAsync(int projectId);
    Task<IEnumerable<string>> GetMemberUserIdsAsync(int projectId);
    Task<bool> ExistsAsync(int projectId, string userId);
    Task AddAsync(ProjectMember member);
    Task<ProjectMember?> FindAsync(int projectId, string userId);
    Task RemoveAsync(ProjectMember member);
}
