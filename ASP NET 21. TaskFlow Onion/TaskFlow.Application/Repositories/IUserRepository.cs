using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetOrderedByEmailExceptIdsAsync(IEnumerable<string> excludeIds);
}
