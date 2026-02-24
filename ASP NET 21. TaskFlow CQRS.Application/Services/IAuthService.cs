using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequest request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request);
    Task RevokeRefreshTokenAsync(RefreshTokenRequest request);
}
