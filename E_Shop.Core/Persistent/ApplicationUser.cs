using E_Shop.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace E_Shop.Core.Persistent;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string Adress { get; set; } = string.Empty;
    public DateTime? LastLogin { get; set; }
    public bool IsDisabled { get; set; }
    public List<RefreshTokens> RefreshTokens { get; set; } = [];
}
// Adress ResturnatName Name 
