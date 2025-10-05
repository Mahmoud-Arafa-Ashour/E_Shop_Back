using E_Shop.Models.Customes.Contracts.login;
using FluentValidation;

namespace E_Shop.Models.Custumes.Validations
{
    public class RefreshTokenRequestValidatot : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidatot()
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
