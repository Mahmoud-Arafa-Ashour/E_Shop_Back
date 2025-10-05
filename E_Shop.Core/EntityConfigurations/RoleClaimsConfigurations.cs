using E_Shop.Core.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Shop.Core.EntityConfigurations;

public class RoleClaimsConfigurations : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        var Permission = Permissions.GetAllPermissions;
        var adminClaims = new List<IdentityRoleClaim<string>>();
        for (int i = 0; i < Permission.Count; i++)
        {
            adminClaims.Add(new IdentityRoleClaim<string>()
            {
                Id = i + 1,
                ClaimType = Permissions.Type,
                ClaimValue = Permission[i],
                RoleId = DefaultRoles.AdminRoleId,
            });
        }
        adminClaims.Add(new IdentityRoleClaim<string>()
        {
            Id = Permission.Count + 1,
            ClaimType = Permissions.Type,
            ClaimValue = Permissions.Info,
            RoleId = DefaultRoles.OwnerRoleId,
        });
        builder.HasData(adminClaims);
    }
}


