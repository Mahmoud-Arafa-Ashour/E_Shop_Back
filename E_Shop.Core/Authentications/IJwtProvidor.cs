using E_Shop.Core.Persistent;

namespace E_Shop.Core.Authentications
{
    public interface IJwtProvidor
    {
        (string token, int expirein) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
        string? ValidateToken(string token);
    }
}
