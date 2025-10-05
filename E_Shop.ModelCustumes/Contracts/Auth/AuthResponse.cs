namespace E_Shop.Models.Customes.Contracts.Auth
{
    public record AuthResponse
        (string id ,
        string email ,
        string Name ,
        string Adress ,
        string PhoneNumber,
        string token ,
        int expiresin,
        string RefreshToken,
        DateTime RefeshTokenExpiration,
        DateTime? LastLogIn 
        );
}
