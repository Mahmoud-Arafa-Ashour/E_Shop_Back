using FluentValidation;

namespace E_Shop.Models.Customes.Contracts.Auth;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required.");
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.");
    }
}
