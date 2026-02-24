using ASP_NET_21._TaskFlow_CQRS.Application.Repositories;
using ASP_NET_21._TaskFlow_CQRS.Domain;
using ASP_NET_21._TaskFlow_CQRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace ASP_NET_21._TaskFlow_CQRS.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskFlowDbContext _context;

    public UserRepository(TaskFlowDbContext context) => _context = context;

    public async Task<IEnumerable<ApplicationUser>> GetOrderedByEmailExceptIdsAsync(IEnumerable<string> excludeIds) =>
        await _context.Users.Where(u => !excludeIds.Contains(u.Id)).OrderBy(u => u.Email).ToListAsync();
}
