using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Common;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Auth_DTOs;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="registerRequest">User registration data (email, password, etc.)</param>
    /// <returns>
    /// Authentication response containing access and refresh tokens.
    /// </returns>
    /// <response code="200">User successfully registered</response>
    /// <response code="400">Invalid registration data</response>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(
        [FromBody] RegisterRequest registerRequest)
    {
        var result = await _authService.RegisterAsync(registerRequest);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }


    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// </summary>
    /// <param name="loginRequest">User login credentials (email and password)</param>
    /// <returns>
    /// Authentication response containing access and refresh tokens.
    /// </returns>
    /// <response code="200">User successfully authenticated</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody]LoginRequest loginRequest)
    {
        var result = await _authService.LoginAsync(loginRequest);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenRequest);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }

    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
         await _authService.RevokeRefreshTokenAsync(refreshTokenRequest.RefreshToken);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse("Refresh token revoked"));
    }
}
