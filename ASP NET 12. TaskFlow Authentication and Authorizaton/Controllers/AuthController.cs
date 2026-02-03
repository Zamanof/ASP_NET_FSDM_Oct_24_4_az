using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Common;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.DTOs.Auth_DTOs;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Controllers;

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
}
