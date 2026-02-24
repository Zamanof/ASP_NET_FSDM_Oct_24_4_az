namespace ASP_NET_21._TaskFlow_CQRS.Application.Contracts.Auth;

public interface IJwtService
{
    (string AccessToken, DateTimeOffset ExpiresAt) GenerateAccessToken(string userId, string email, IList<string> roles);
    Task<(string RefreshJwt, DateTimeOffset ExpiresAt)> CreateAndStoreRefreshTokenAsync(string userId);
    (string UserId, string Jti) ValidateRefreshToken(string refreshJwt, bool validateLifetime = true);
    string GetJtiFromRefreshToken(string refreshJwt);
}
