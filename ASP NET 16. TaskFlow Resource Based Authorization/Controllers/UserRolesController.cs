using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Common;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Auth_DTOs;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy ="AdminOnly")]
public class UserRolesController : ControllerBase
{

    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRolesController(
        RoleManager<IdentityRole> roleManager, 
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserWithRolesDto>>>> GetAll()
    {
        var users = _userManager.Users.OrderBy(u=> u.Email).ToList();

        var userWithRolesList = new List<UserWithRolesDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userWithRolesList.Add(new UserWithRolesDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            });
        }

        return Ok(ApiResponse<IEnumerable<UserWithRolesDto>>.SuccessResponse(userWithRolesList, "Users list"));
    }

    [HttpGet("{userId}/roles")]
    public async Task<ActionResult<ApiResponse<UserWithRolesDto>>> GetRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound(ApiResponse<IList<string>>.ErrorResponse($"User with ID {userId} not found"));
        var roles = await _userManager.GetRolesAsync(user);

        var userWithRoles = new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };
        return Ok(ApiResponse<UserWithRolesDto>.SuccessResponse(userWithRoles, $"Roles for user {user.Id}"));
    }

    [HttpPost("{userId}/roles")]
    public async Task<ActionResult<ApiResponse<UserWithRolesDto>>> AssignRole(string userId,[FromBody] AssignRoleRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    public async Task<ActionResult<ApiResponse<UserWithRolesDto>>> RemoveRole(string userId, string roleName)
    {
        throw new NotImplementedException();
    }
}