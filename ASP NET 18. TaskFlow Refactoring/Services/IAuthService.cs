using ASP_NET_18._TaskFlow_Refactoring.DTOs.Auth_DTOs;

namespace ASP_NET_18._TaskFlow_Refactoring.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest);
    Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    Task RevokeRefreshTokenAsync(string refreshToken);

}
