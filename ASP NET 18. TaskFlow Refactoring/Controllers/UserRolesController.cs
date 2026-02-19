using ASP_NET_18._TaskFlow_Refactoring.Common;
using ASP_NET_18._TaskFlow_Refactoring.DTOs;
using ASP_NET_18._TaskFlow_Refactoring.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NET_18._TaskFlow_Refactoring.Controllers;

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
        var roleName = request.Role.Trim();

        if (string.IsNullOrEmpty(roleName))
            return BadRequest(ApiResponse<UserWithRolesDto>.ErrorResponse("Role name cannot be empty"));

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound(ApiResponse<UserWithRolesDto>.ErrorResponse($"User with ID {userId} not found"));

        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
            return NotFound(ApiResponse<UserWithRolesDto>.ErrorResponse($"Role {roleName} does not exist"));

        if (await _userManager.IsInRoleAsync(user, roleName))
            return BadRequest(ApiResponse<UserWithRolesDto>.ErrorResponse($"User {user.Email} is already in role {roleName}"));

        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<UserWithRolesDto>.ErrorResponse($"Failed to assign role {roleName} to user {user.Email}"));

        var roles = await _userManager.GetRolesAsync(user);

        var userWithRoles = new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };

        return Ok(ApiResponse<UserWithRolesDto>.SuccessResponse(userWithRoles, $"Role {roleName} assigned to user {user.Email}"));
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    public async Task<ActionResult<ApiResponse<UserWithRolesDto>>> RemoveRole(string userId, string roleName)
    {
        roleName = roleName.Trim();

        if (string.IsNullOrEmpty(roleName))
            return BadRequest(ApiResponse<UserWithRolesDto>.ErrorResponse("Role name cannot be empty"));

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound(ApiResponse<UserWithRolesDto>.ErrorResponse($"User with ID {userId} not found"));

        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
            return NotFound(ApiResponse<UserWithRolesDto>.ErrorResponse($"Role {roleName} does not exist"));

        if (!await _userManager.IsInRoleAsync(user, roleName))
            return BadRequest(ApiResponse<UserWithRolesDto>.ErrorResponse($"User {user.Email} not in role {roleName}"));

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<UserWithRolesDto>.ErrorResponse($"Failed to remove role {roleName} to user {user.Email}"));

        var roles = await _userManager.GetRolesAsync(user);

        var userWithRoles = new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList()
        };

        return Ok(ApiResponse<UserWithRolesDto>.SuccessResponse(userWithRoles, $"Role {roleName} assigned to user {user.Email}"));
    }
}