namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Auth_DTOs;

public class RegisterRequest
{
    /// <summary>
    /// FirstName
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// LastName
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Password
    /// </summary>
    /// <example>Pass123</example>
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// Confirmed Password
    /// </summary>
    /// <example>Pass123</example>
    public string ConfirmPassword { get; set; } = string.Empty;

}
public class LoginRequest
{
    /// <summary>
    /// Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Password
    /// </summary>
    /// <example>Pass123</example>
    public string Password { get; set; } = string.Empty;
}
public class AuthResponseDto
{
    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    /// <summary>
    /// Token Expires date 
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }
    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    /// <summary>
    /// RefreshToken Expires date 
    /// </summary>
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
    /// <summary>
    /// User Email
    /// </summary>
    /// <example>john@doe.com</example>
    public string Email { get; set; } = string.Empty;

    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}