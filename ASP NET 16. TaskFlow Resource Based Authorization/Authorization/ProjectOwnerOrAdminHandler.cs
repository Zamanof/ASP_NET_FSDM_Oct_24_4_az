using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Authorization;

public class ProjectOwnerOrAdminHandler
    : AuthorizationHandler<ProjectOwnerOrAdminRequirment, Project>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectOwnerOrAdminRequirment requirement, Project resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Task.CompletedTask;

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (context.User.IsInRole("Manager") && resource.OwnerId == userId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
