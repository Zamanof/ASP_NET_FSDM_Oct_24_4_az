using ASP_NET_20._TaskFlow.Models;

namespace ASP_NET_20._TaskFlow.DAL;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetOrderedByEmailExceptIdsAsync(IEnumerable<string> excludeIds);
}
