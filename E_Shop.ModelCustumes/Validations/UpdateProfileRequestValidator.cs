using E_Shop.Models.Custumes.Contracts.User;
using FluentValidation;

namespace E_Shop.Models.Custumes.Validations
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        //string Name, string Adress, string PhoneNumber, string PharmacyName
        public UpdateProfileRequestValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty();
            RuleFor(x => x.Adress)
                .NotEmpty();
            RuleFor(x => x.PharmacyName)
                .NotEmpty();
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Length(11 , 100)
                .WithMessage("Can not be less than 11 digits");
        }
    }
}
