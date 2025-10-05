using E_Shop.Core.Abstractions;
using E_Shop.Core.Persistent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Shop.Core.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
        new ApplicationRole
          {
              Id = DefaultRoles.AdminRoleId,
              Name = DefaultRoles.Admin,
              NormalizedName = DefaultRoles.Admin.ToUpper(),
              ConcurrencyStamp = DefaultRoles.AdminConcurencyStamp,
              isDefault = false,
              IsDeleted = false
          }
        , new ApplicationRole
        {
            Id = DefaultRoles.OwnerRoleId,
            Name = DefaultRoles.Owner,
            NormalizedName = DefaultRoles.Owner.ToUpper(),
            ConcurrencyStamp = DefaultRoles.OwnerConcurencyStamp,
            isDefault = true,
            IsDeleted = false
        }
        );
    }
}


