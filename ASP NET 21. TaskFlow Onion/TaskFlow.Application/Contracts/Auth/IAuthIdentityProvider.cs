using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Contracts.Auth;

public interface IAuthIdentityProvider
{
    Task<AuthUserInfo?> FindByEmailAsync(string email);
    Task<AuthUserInfo?> FindByIdAsync(string id);
    Task<AuthUserInfo> CreateAsync(RegisterRequest request);
    Task<bool> CheckPasswordAsync(string userId, string password);
    Task<IList<string>> GetRolesAsync(string userId);
    Task AddToRoleAsync(string userId, string role);
}
