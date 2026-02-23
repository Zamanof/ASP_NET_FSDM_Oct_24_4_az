using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.Config;
using TaskFlow.Application.Contracts.Auth;
using TaskFlow.Application.Repositories;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Services;

public class JwtService : IJwtService
{
    private const string RefreshTokenType = "refresh";
    private readonly JwtConfig _jwtConfig;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public JwtService(IOptions<JwtConfig> jwtConfig, IRefreshTokenRepository refreshTokenRepo)
    {
        _jwtConfig = jwtConfig.Value;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public (string AccessToken, DateTimeOffset ExpiresAt) GenerateAccessToken(string userId, string email, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes);
        var token = new JwtSecurityToken(
            _jwtConfig.Issuer,
            _jwtConfig.Audience,
            claims,
            notBefore: null,
            expires: expiresAt,
            signingCredentials: credentials);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, DateTimeOffset.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes));
    }

    public async Task<(string RefreshJwt, DateTimeOffset ExpiresAt)> CreateAndStoreRefreshTokenAsync(string userId)
    {
        var jti = Guid.NewGuid().ToString("N");
        var expiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpirationDays);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId), new(JwtRegisteredClaimNames.Jti, jti), new(JwtRegisteredClaimNames.Sub, userId), new("token_type", RefreshTokenType) };
        var token = new JwtSecurityToken(_jwtConfig.Issuer, _jwtConfig.Audience, claims, expires: expiresAt, signingCredentials: credentials);
        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);
        var entity = new RefreshToken { JwtId = jti, UserId = userId, ExpiresAt = expiresAt, CreatedAt = DateTime.UtcNow };
        await _refreshTokenRepo.AddAsync(entity);
        return (jwtString, expiresAt);
    }

    public (string UserId, string Jti) ValidateRefreshToken(string refreshJwt, bool validateLifetime = true)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey!));
        var principal = handler.ValidateToken(refreshJwt, new TokenValidationParameters
        {
            ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = validateLifetime, ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfig.Issuer, ValidAudience = _jwtConfig.Audience, IssuerSigningKey = key, ClockSkew = TimeSpan.Zero
        }, out var validatedToken);
        if (validatedToken is not JwtSecurityToken jwt || jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value != RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid refresh token");
        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? throw new UnauthorizedAccessException("Invalid refresh token");
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Invalid refresh token");
        return (userId, jti);
    }

    public string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();
        return !handler.CanReadToken(refreshJwt) ? "" : (handler.ReadJwtToken(refreshJwt).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? "");
    }
}
