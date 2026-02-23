using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequest request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request);
    Task RevokeRefreshTokenAsync(RefreshTokenRequest request);
}
