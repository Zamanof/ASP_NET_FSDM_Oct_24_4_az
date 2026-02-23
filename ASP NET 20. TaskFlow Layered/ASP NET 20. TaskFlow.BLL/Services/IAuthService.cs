using ASP_NET_20._TaskFlow.BLL.DTOs;

namespace ASP_NET_20._TaskFlow.BLL.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequest request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request);
    Task RevokeRefreshTokenAsync(RefreshTokenRequest request);
}
