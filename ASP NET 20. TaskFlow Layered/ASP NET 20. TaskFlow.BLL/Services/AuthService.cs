using ASP_NET_20._TaskFlow.BLL.Config;
using ASP_NET_20._TaskFlow.BLL.DTOs;
using ASP_NET_20._TaskFlow.DAL;
using ASP_NET_20._TaskFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP_NET_20._TaskFlow.BLL.Services;

public class AuthService : IAuthService
{
    private const string RefreshTokenType = "refresh";
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly JwtConfig _jwtConfig;

    public AuthService(UserManager<ApplicationUser> userManager, IRefreshTokenRepository refreshTokenRepo, IOptions<JwtConfig> jwtConfig)
    {
        _userManager = userManager;
        _refreshTokenRepo = refreshTokenRepo;
        _jwtConfig = jwtConfig.Value;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new UnauthorizedAccessException("Invalid email or password.");
        if (!await _userManager.CheckPasswordAsync(user, request.Password)) throw new UnauthorizedAccessException("Invalid email or password.");
        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null) throw new InvalidOperationException("User with this email already exists.");
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FirstName = request.FirstName, LastName = request.LastName, CreatedAt = DateTime.UtcNow, UpdatedAt = null };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new InvalidOperationException($"User creation failed: {string.Join(",", result.Errors.Select(e => e.Description))}");
        await _userManager.AddToRoleAsync(user, "User");
        return await GenerateTokenAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var (principal, jti) = ValidateRefreshJwtAndGetJti(request.RefreshToken);
        var storedToken = await _refreshTokenRepo.GetByJwtIdAsync(jti);
        if (storedToken == null || !storedToken.IsActive) throw new UnauthorizedAccessException("Invalid refresh token");
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null) throw new UnauthorizedAccessException("User not found");
        storedToken.RevokedAt = DateTime.UtcNow;
        var newTokens = await GenerateTokenAsync(user);
        var newStored = await _refreshTokenRepo.GetByJwtIdAsync(GetJtiFromRefreshToken(newTokens.RefreshToken));
        if (newStored != null) storedToken.ReplacedByJwtId = newStored.JwtId;
        await _refreshTokenRepo.UpdateAsync(storedToken);
        return newTokens;
    }

    public async Task RevokeRefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var (_, jti) = ValidateRefreshJwtAndGetJti(request.RefreshToken, false);
            var storedToken = await _refreshTokenRepo.GetByJwtIdAsync(jti);
            if (storedToken != null && storedToken.IsActive) { storedToken.RevokedAt = DateTime.UtcNow; await _refreshTokenRepo.UpdateAsync(storedToken); }
        }
        catch { }
    }

    private async Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
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
        var (refreshToken, refreshJwt) = await CreateRefreshTokenJwtAsync(user.Id, _jwtConfig.RefreshTokenExpirationDays);
        return new AuthResponseDto
        {
            AccessToken = tokenString,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            RefreshToken = refreshJwt,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            Email = user.Email ?? "",
            Roles = roles
        };
    }

    private (ClaimsPrincipal principal, string jti) ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifetime = true)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey!));
        var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = validateLifetime, ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfig.Issuer, ValidAudience = _jwtConfig.Audience, IssuerSigningKey = key, ClockSkew = TimeSpan.Zero
        }, out var validatedToken);
        if (validatedToken is not JwtSecurityToken jwt || jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value != RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid refresh token");
        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? throw new UnauthorizedAccessException("Invalid refresh token");
        return (principal, jti);
    }

    private async Task<(RefreshToken entity, string jwt)> CreateRefreshTokenJwtAsync(string userId, int expirationDays)
    {
        var jti = Guid.NewGuid().ToString("N");
        var expiresAt = DateTime.UtcNow.AddDays(expirationDays);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId), new(JwtRegisteredClaimNames.Jti, jti), new(JwtRegisteredClaimNames.Sub, userId), new("token_type", RefreshTokenType) };
        var token = new JwtSecurityToken(_jwtConfig.Issuer, _jwtConfig.Audience, claims, expires: expiresAt, signingCredentials: credentials);
        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);
        var entity = new RefreshToken { JwtId = jti, UserId = userId, ExpiresAt = expiresAt, CreatedAt = DateTime.UtcNow };
        await _refreshTokenRepo.AddAsync(entity);
        return (entity, jwtString);
    }

    private static string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();
        return !handler.CanReadToken(refreshJwt) ? "" : (handler.ReadJwtToken(refreshJwt).Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? "");
    }
}
