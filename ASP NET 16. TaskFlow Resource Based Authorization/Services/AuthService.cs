using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Data;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Auth_DTOs;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Services;

public class AuthService : IAuthService
{

    private const string RefreshTokenType = "refresh";
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly TaskFlowDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        TaskFlowDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        await _userManager.AddToRoleAsync(user, "User");
        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var (principal, jti) = ValidateRefreshJwtAndGetJti(request.RefreshToken);

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);
        if (storedToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");
        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token has been revoked or expired.");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? storedToken.UserId;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found.");


        storedToken.RevokedAt = DateTime.UtcNow;

        var newTokens = await GenerateTokensAsync(user);
        var newStored = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.JwtId == GetJtiFromRefreshToken(newTokens.RefreshToken));
        if (newStored != null)
            storedToken.ReplacedByJwtId = newStored.JwtId;

        await _context.SaveChangesAsync();
        return newTokens;
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        string? jti;
        try
        {
            (_, jti) = ValidateRefreshJwtAndGetJti(refreshToken, validateLifetime: false);
        }
        catch
        {
            return;
        }

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.JwtId == jti);
        if (storedToken == null || !storedToken.IsActive)
            return;

        storedToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> GenerateTokensAsync(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var accessExpirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"]!);
        var refreshExpirationDays = int.Parse(jwtSettings["RefreshTokenExpirationDays"]!);

        var accessToken = await GenerateAccessTokenAsync(user, accessExpirationMinutes);
        var (refreshEntity, refreshJwt) = await CreateRefreshTokenJwtAsync(user.Id, refreshExpirationDays);

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(accessExpirationMinutes),
            RefreshToken = refreshJwt,
            RefreshTokenExpiresAt = refreshEntity.ExpiresAt,
            Email = user.Email ?? string.Empty,
            Roles = roles
        };
    }

    private async Task<string> GenerateAccessTokenAsync(ApplicationUser user, int expirationMinutes)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        var issuer = jwtSettings["Issuer"] ?? "TaskFlow";
        var audience = jwtSettings["Audience"] ?? "TaskFlowUsers";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<(RefreshToken entity, string jwt)> CreateRefreshTokenJwtAsync(string userId, int expirationDays)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var refreshSecretKey = jwtSettings["RefreshTokenSecretKey"]
            ?? throw new InvalidOperationException("JWT RefreshTokenSecretKey or SecretKey is not configured.");
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var jti = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddDays(expirationDays);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("token_type", RefreshTokenType)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

        var entity = new RefreshToken
        {
            JwtId = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync();

        return (entity, jwtString);
    }

    private (ClaimsPrincipal principal, string jti) ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifetime = true)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var refreshSecretKey = jwtSettings["RefreshTokenSecretKey"]
            ?? throw new InvalidOperationException("JWT RefreshTokenSecretKey or SecretKey is not configured.");
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshSecretKey));

        var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        var tokenType = jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
        if (tokenType != RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid token type.");

        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        return (principal, jti);
    }

    private static string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(refreshJwt))
            return string.Empty;
        var jwt = handler.ReadJwtToken(refreshJwt);
        return jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    }
}