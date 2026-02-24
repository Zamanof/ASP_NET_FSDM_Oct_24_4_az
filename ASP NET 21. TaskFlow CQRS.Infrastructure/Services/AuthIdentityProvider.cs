using ASP_NET_21._TaskFlow_CQRS.Application.Contracts.Auth;
using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_21._TaskFlow_CQRS.Domain;
using Microsoft.AspNetCore.Identity;

namespace ASP_NET_21._TaskFlow_CQRS.Infrastructure.Services;

public class AuthIdentityProvider : IAuthIdentityProvider
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthIdentityProvider(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<AuthUserInfo?> FindByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null ? null : ToAuthUserInfo(user);
    }

    public async Task<AuthUserInfo?> FindByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user == null ? null : ToAuthUserInfo(user);
    }

    public async Task<AuthUserInfo> CreateAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"User creation failed: {string.Join(",", result.Errors.Select(e => e.Description))}");
        return ToAuthUserInfo(user);
    }

    public async Task<bool> CheckPasswordAsync(string userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IList<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? new List<string>() : await _userManager.GetRolesAsync(user);
    }

    public async Task AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null) await _userManager.AddToRoleAsync(user, role);
    }

    private static AuthUserInfo ToAuthUserInfo(ApplicationUser u) =>
        new() { Id = u.Id, Email = u.Email ?? "", UserName = u.UserName ?? "", FirstName = u.FirstName, LastName = u.LastName };
}
