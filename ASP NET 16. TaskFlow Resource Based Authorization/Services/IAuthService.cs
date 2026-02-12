using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Auth_DTOs;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest);
    Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    Task RevokeRefreshTokenAsync(string refreshToken);

}
