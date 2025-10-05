using E_Shop.Models.Customes.Contracts.login;
using FluentValidation;

namespace E_Shop.Models.Custumes.Validations
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator() 
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
