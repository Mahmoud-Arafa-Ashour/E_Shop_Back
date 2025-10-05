using Microsoft.AspNetCore.Authorization;

namespace E_Shop.Core.Authorization;

public class PermissionRequiremen(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
