using E_Shop.Core.Abstractions;
using E_Shop.Core.Persistent;
using E_Shop.Models.Custumes.Contracts.Role;
using E_Shop.Service.IServices;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.RelationalQueryableExtensions;
using static E_Shop.Core.Abstractions.Errors;

namespace E_Shop.Service.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager , ApplicationDbContext dbContext) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync(CancellationToken cancellationToken) =>
        await _roleManager.Roles
        .Where(x => !x.isDefault)
        .ProjectToType<RoleResponse>()
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<RoleResponse>> GetActiveRoles(CancellationToken cancellationToken) =>
        await _roleManager.Roles
        .Where(x => !x.IsDeleted)
        .ProjectToType<RoleResponse>()
        .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailsResponse>> GetRoleDetails(string RoleId , CancellationToken cancellationToken)
    {
       var role =  await _roleManager.FindByIdAsync(RoleId);
        if(role is null)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.NotFound);

        var permissions = await _roleManager.GetClaimsAsync(role!);

        var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(_ => _.Value));
        return Result.Success(response);
    }

    public async Task<Result<RoleDetailsResponse>> AddRoleAsync(RoleRequest roleRequest, CancellationToken cancellationToken)
    {
        var isExistRole = await _roleManager.RoleExistsAsync(roleRequest.Name);
        if (isExistRole)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.DuplicateRole);
        var ExistedPermissions = Permissions.GetAllPermissions;
        if (roleRequest.Permissions.Except(ExistedPermissions).Any())
            return Result.Failure<RoleDetailsResponse>(RoleErrors.NotValid);
        var role = new ApplicationRole()
        {
            Name = roleRequest.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        var result = await _roleManager.CreateAsync(role);
        if (result.Succeeded)
        {
            var Permission = roleRequest.Permissions.Select(x => new IdentityRoleClaim<string>()
            {
                ClaimType = Permissions.Type,
                ClaimValue = x,
                RoleId = role.Id
            });
            await _dbContext.AddRangeAsync(Permission, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            var response = new RoleDetailsResponse(role.Id, role.Name, role.isDefault, roleRequest.Permissions);
            return Result.Success(response);
        }
        var error = result.Errors.First();
        return Result.Failure<RoleDetailsResponse>(new Error(error!.Code, error.Description, StatusCodes.Status409Conflict));
    }
    public async Task<Result> UpdateAsync(string id, RoleRequest request)
    {
        var roleIsExists = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id);

        if (roleIsExists)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.DuplicateRole);

        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.NotFound);

        var allowedPermissions = Permissions.GetAllPermissions;

        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailsResponse>(RoleErrors.NotValid);

        role.Name = request.Name;

        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            var currentPermissions = await _dbContext.RoleClaims
                .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                .Select(x => x.ClaimValue)
                .ToListAsync();

            var newPermissions = request.Permissions.Except(currentPermissions)
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });

            var removedPermissions = currentPermissions.Except(request.Permissions);

            await Microsoft.EntityFrameworkCore.RelationalQueryableExtensions.ExecuteDeleteAsync(
                _dbContext.RoleClaims
                    .Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue)));


            await _dbContext.AddRangeAsync(newPermissions);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure<RoleDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ToggleStatusAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.NotFound);

        role.IsDeleted = !role.IsDeleted;

        await _roleManager.UpdateAsync(role);

        return Result.Success();
    }

    public async Task<Result<RoleResponse>> Get(string id, CancellationToken cancellationToken)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure<RoleResponse>(RoleErrors.NotFound);
        var response = role.Adapt<RoleResponse>();
        return Result.Success(response);
    }
}
