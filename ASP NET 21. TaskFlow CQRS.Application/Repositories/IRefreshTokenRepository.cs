using ASP_NET_21._TaskFlow_CQRS.Domain;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByJwtIdAsync(string jwtId);
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
}
