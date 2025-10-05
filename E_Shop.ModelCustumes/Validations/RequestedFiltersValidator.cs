using E_Shop.Core.Abstractions;
using FluentValidation;

namespace E_Shop.Models.Customes;

public class RequestedFiltersValidator : AbstractValidator<RequestedFilters>
{
    public RequestedFiltersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");
        RuleFor(x => x.PageSize)
            .GreaterThan(10)
            .WithMessage("Page size must be greater than 10");
    }
}
