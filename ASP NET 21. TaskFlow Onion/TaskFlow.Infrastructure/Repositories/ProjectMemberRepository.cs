using TaskFlow.Domain.Entities;
using TaskFlow.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

public class ProjectMemberRepository : IProjectMemberRepository
{
    private readonly TaskFlowDbContext _context;

    public ProjectMemberRepository(TaskFlowDbContext context) => _context = context;

    public async Task AddAsync(ProjectMember member) { _context.ProjectMembers.Add(member); await _context.SaveChangesAsync(); }

    public async Task<bool> ExistsAsync(int projectId, string userId) =>
        await _context.ProjectMembers.AnyAsync(m => m.ProjectId == projectId && m.UserId == userId);

    public async Task<ProjectMember?> FindAsync(int projectId, string userId) =>
        await _context.ProjectMembers.FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);

    public async Task<IEnumerable<string>> GetMemberUserIdsAsync(int projectId) =>
        await _context.ProjectMembers.Where(m => m.ProjectId == projectId).Select(m => m.UserId).ToListAsync();

    public async Task<IEnumerable<ProjectMember>> GetByProjectIdWithUserAsync(int projectId) =>
        await _context.ProjectMembers.Include(m => m.User).Where(m => m.ProjectId == projectId).OrderBy(m => m.CreatedAt).ToListAsync();

    public async Task RemoveAsync(ProjectMember member) { _context.ProjectMembers.Remove(member); await _context.SaveChangesAsync(); }
}
