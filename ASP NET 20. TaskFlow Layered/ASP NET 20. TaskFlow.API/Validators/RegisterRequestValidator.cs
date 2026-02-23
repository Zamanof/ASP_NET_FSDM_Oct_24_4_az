using ASP_NET_20._TaskFlow.BLL.DTOs;
using FluentValidation;

namespace ASP_NET_20._TaskFlow.API.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required").MinimumLength(2).WithMessage("FirstName must be at least 2 characters");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required").MinimumLength(2).WithMessage("LastName must be at least 2 characters");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email is not valid");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").MinimumLength(6).WithMessage("Password must be at least 6 characters").Password().WithMessage("Password must contain at least one digit, one lowercase and one uppercase.");
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("ConfirmPassword is required").Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}
