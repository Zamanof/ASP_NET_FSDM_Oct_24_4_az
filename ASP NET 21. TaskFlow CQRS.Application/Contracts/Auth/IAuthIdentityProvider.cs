using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Contracts.Auth;

public interface IAuthIdentityProvider
{
    Task<AuthUserInfo?> FindByEmailAsync(string email);
    Task<AuthUserInfo?> FindByIdAsync(string id);
    Task<AuthUserInfo> CreateAsync(RegisterRequest request);
    Task<bool> CheckPasswordAsync(string userId, string password);
    Task<IList<string>> GetRolesAsync(string userId);
    Task AddToRoleAsync(string userId, string role);
}
