using E_Shop.Models.Custumes.Contracts.Product;
using FluentValidation;

namespace E_Commerce.Contracts.Product
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 100);
            RuleFor(x => x.Description)
                .NotEmpty();
            RuleFor(x => x.Price)
                .NotEmpty();
        }
    }
}
