using System.Text.RegularExpressions;

namespace E_Shop.Models.Custumes.Contracts.User
{
    public record ChangePasswordRequset(string CurrentPassword ,string NewPassword);
}
