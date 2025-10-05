using E_Shop.Core.Abstractions;
using E_Shop.Core.Persistent;
using E_Shop.Models.Custumes.Contracts.UserRole;
using E_Shop.Service.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using static E_Shop.Core.Abstractions.Errors;

namespace E_Shop.Service.Services;

public class UserRoleService(UserManager<ApplicationUser> userManager , RoleManager<ApplicationRole> roleManager) : IUserRoleService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result> AssignRole(UserRoleRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Result.Failure(UserRoleErrors.EmptyUserId);

        if (string.IsNullOrWhiteSpace(request.RoleName))
            return Result.Failure(UserRoleErrors.EmptyRoleName);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure(UserRoleErrors.UserNotFound);

        var role = await _roleManager.FindByNameAsync(request.RoleName);
        if (role is null)
            return Result.Failure(UserRoleErrors.RoleNotFound);

        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles.Contains(request.RoleName))
            return Result.Failure(UserRoleErrors.AlreadyInRole);

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (result.Succeeded)
            return Result.Success();

        var errors = result.Errors.First();
        return Result.Failure(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));
    }
}
