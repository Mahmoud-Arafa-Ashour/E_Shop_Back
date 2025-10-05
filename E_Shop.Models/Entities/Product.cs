namespace E_Shop.Models.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int MinimumQuantity { get; set; }
    public double DiscountRate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
    public string Category { get; set; } = string.Empty;
}
