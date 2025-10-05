using System.Security.Claims;

namespace E_Shop.Core.Abstractions
{
    public static class UserExtentions
    {
        public static string? GetUserID(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
