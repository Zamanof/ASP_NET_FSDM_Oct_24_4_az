using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Auth_DTOs;
using FluentValidation;


namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required")
            .MinimumLength(2).WithMessage("Firstname must be at least 2 characters long");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required")
            .MinimumLength(2).WithMessage("Lastname must be at least 2 characters long");
        
        RuleFor(x=> x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Passwords must be at least 6 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Passwords must have at least one digit ('0'-'9').,Passwords must have at least one lowercase ('a'-'z').,Passwords must have at least one uppercase ('A'-'Z')");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirmed is required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");

    }
}

public class LoginRequestValidator: AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x=> x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

    RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Passwords must be at least 6 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Passwords must have at least one digit ('0'-'9').,Passwords must have at least one lowercase ('a'-'z').,Passwords must have at least one uppercase ('A'-'Z')");
    }
    
}
