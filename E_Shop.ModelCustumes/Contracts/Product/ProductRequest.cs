using Microsoft.AspNetCore.Http;

namespace E_Shop.Models.Custumes.Contracts.Product
{
    public record ProductRequest(string Name, string Description , decimal Price , IFormFile? Image);
}
