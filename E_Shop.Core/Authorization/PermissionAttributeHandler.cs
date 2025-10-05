
using E_Shop.Core.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace E_Shop.Core.Authorization;

public class PermissionAttributeHandler : AuthorizationHandler<PermissionRequiremen>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequiremen requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
            return Task.CompletedTask;

        // Check for permissions claim
        var permissionsClaim = context.User.Claims.FirstOrDefault(x => x.Type == "permissions");
        if (permissionsClaim != null)
        {
            try
            {
                var permissions = JsonSerializer.Deserialize<string[]>(permissionsClaim.Value);
                if (permissions != null && permissions.Contains(requirement.Permission))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            catch (JsonException)
            {
                // If JSON deserialization fails, try direct string comparison
                if (permissionsClaim.Value == requirement.Permission)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
        }

        // Fallback: check for direct claim match (for backward compatibility)
        if (context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
