using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.DTOs.Auth_DTOs;
using ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
<<<<<<< HEAD
using System.Threading.Tasks;
=======
>>>>>>> fcc8b3cbf15e0de21c2f5a46f536e8db4ceb024a

namespace ASP_NET_12._TaskFlow_Authentication_and_Authorizaton.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
<<<<<<< HEAD
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
=======
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
>>>>>>> fcc8b3cbf15e0de21c2f5a46f536e8db4ceb024a
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

<<<<<<< HEAD
        if(!isValidPassword)
            throw new UnauthorizedAccessException("Invalid login or password");

        return await CreateTokenAsycn(user);
=======
        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
        return await GenerateTokenAsync(user);
>>>>>>> fcc8b3cbf15e0de21c2f5a46f536e8db4ceb024a
    }

  

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest registerRequest)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));

            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "User");

<<<<<<< HEAD
        return await CreateTokenAsycn(user);
    }
    private async Task<AuthResponseDto> CreateTokenAsycn(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JWTSettings");
        var secretKey = jwtSettings["SecretKey"];
        var audience = jwtSettings["Audience"];
        var issuer = jwtSettings["Issuer"];
        var expirationInMinutes = int.Parse(jwtSettings["ExpirationInMinutes"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

=======
        return await GenerateTokenAsync(user);
    }

    private async Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var roles = await _userManager.GetRolesAsync(user);


        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };


>>>>>>> fcc8b3cbf15e0de21c2f5a46f536e8db4ceb024a
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
<<<<<<< HEAD
            expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
            signingCredentials: credentials           
            );
=======
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );
>>>>>>> fcc8b3cbf15e0de21c2f5a46f536e8db4ceb024a

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            AccessToken = tokenString,
<<<<<<< HEAD
            ExpiredAt = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            Email = user.Email!,
=======
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Email = user.Email ?? string.Empty,
>>>>>>> fcc8b3cbf15e0de21c2f5a46f536e8db4ceb024a
            Roles = roles
        };
    }
}
