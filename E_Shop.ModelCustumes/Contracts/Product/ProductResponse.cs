namespace E_Shop.Models.Custumes.Contracts.Product
{
    public record ProductResponse(int Id , string Name, string Description , decimal Price , string? ImagePath);
}
