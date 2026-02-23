using TaskFlow.Domain.Entities;
using TaskFlow.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskFlowDbContext _context;

    public UserRepository(TaskFlowDbContext context) => _context = context;

    public async Task<IEnumerable<ApplicationUser>> GetOrderedByEmailExceptIdsAsync(IEnumerable<string> excludeIds) =>
        await _context.Users.Where(u => !excludeIds.Contains(u.Id)).OrderBy(u => u.Email).ToListAsync();
}
