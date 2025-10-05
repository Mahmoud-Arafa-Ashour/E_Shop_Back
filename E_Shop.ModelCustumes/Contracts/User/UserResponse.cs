namespace E_Shop.Models.Custumes.Contracts.User
{
    public record UserResponse(string id , string Email, string Name, string Adress, string PhoneNumber, IEnumerable<string> Roles);
}
//PharmacyName