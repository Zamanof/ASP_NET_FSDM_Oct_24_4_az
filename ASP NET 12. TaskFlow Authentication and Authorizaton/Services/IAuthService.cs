using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.DTOs.Auth_DTOs;

namespace ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest);
    Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest);
}
