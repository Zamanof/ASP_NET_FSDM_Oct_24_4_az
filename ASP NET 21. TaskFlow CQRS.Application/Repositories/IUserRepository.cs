using ASP_NET_21._TaskFlow_CQRS.Domain;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetOrderedByEmailExceptIdsAsync(IEnumerable<string> excludeIds);
}
