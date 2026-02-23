using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Repositories;

public interface IProjectMemberRepository
{
    Task<IEnumerable<ProjectMember>> GetByProjectIdWithUserAsync(int projectId);
    Task<IEnumerable<string>> GetMemberUserIdsAsync(int projectId);
    Task<bool> ExistsAsync(int projectId, string userId);
    Task AddAsync(ProjectMember member);
    Task<ProjectMember?> FindAsync(int projectId, string userId);
    Task RemoveAsync(ProjectMember member);
}
