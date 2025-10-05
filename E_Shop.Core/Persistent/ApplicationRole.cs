using Microsoft.AspNetCore.Identity;

namespace E_Shop.Core.Persistent;

public class ApplicationRole : IdentityRole
{
    public bool isDefault { get; set; }
    public bool IsDeleted { get; set; }
}
