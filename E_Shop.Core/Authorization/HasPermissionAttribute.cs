using Microsoft.AspNetCore.Authorization;

namespace E_Shop.Core.Authorization;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute
{
    public HasPermissionAttribute() : this(string.Empty)
    {
    }
    
    public string Permission => permission;
}
