using TaskFlow.Application.Contracts.Auth;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;

namespace TaskFlow.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthIdentityProvider _authIdentityProvider;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IJwtService _jwtService;

    public AuthService(IAuthIdentityProvider authIdentityProvider, IRefreshTokenRepository refreshTokenRepo, IJwtService jwtService)
    {
        _authIdentityProvider = authIdentityProvider;
        _refreshTokenRepo = refreshTokenRepo;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest request)
    {
        var user = await _authIdentityProvider.FindByEmailAsync(request.Email);
        if (user == null) throw new UnauthorizedAccessException("Invalid email or password.");
        if (!await _authIdentityProvider.CheckPasswordAsync(user.Id, request.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");
        return await GenerateTokenResponseAsync(user);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request)
    {
        if (await _authIdentityProvider.FindByEmailAsync(request.Email) != null)
            throw new InvalidOperationException("User with this email already exists.");
        var user = await _authIdentityProvider.CreateAsync(request);
        await _authIdentityProvider.AddToRoleAsync(user.Id, "User");
        return await GenerateTokenResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var (userId, jti) = _jwtService.ValidateRefreshToken(request.RefreshToken);
        var storedToken = await _refreshTokenRepo.GetByJwtIdAsync(jti);
        if (storedToken == null || !storedToken.IsActive) throw new UnauthorizedAccessException("Invalid refresh token");
        var user = await _authIdentityProvider.FindByIdAsync(userId);
        if (user == null) throw new UnauthorizedAccessException("User not found");
        storedToken.RevokedAt = DateTime.UtcNow;
        var newTokens = await GenerateTokenResponseAsync(user);
        var newJti = _jwtService.GetJtiFromRefreshToken(newTokens.RefreshToken);
        var newStored = await _refreshTokenRepo.GetByJwtIdAsync(newJti);
        if (newStored != null) storedToken.ReplacedByJwtId = newStored.JwtId;
        await _refreshTokenRepo.UpdateAsync(storedToken);
        return newTokens;
    }

    public async Task RevokeRefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var (_, jti) = _jwtService.ValidateRefreshToken(request.RefreshToken, false);
            var storedToken = await _refreshTokenRepo.GetByJwtIdAsync(jti);
            if (storedToken != null && storedToken.IsActive)
            {
                storedToken.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepo.UpdateAsync(storedToken);
            }
        }
        catch { }
    }

    private async Task<AuthResponseDto> GenerateTokenResponseAsync(AuthUserInfo user)
    {
        var roles = await _authIdentityProvider.GetRolesAsync(user.Id);
        var (accessToken, expiresAt) = _jwtService.GenerateAccessToken(user.Id, user.Email, roles);
        var (refreshToken, refreshExpiresAt) = await _jwtService.CreateAndStoreRefreshTokenAsync(user.Id);
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt,
            Email = user.Email,
            Roles = roles
        };
    }
}
