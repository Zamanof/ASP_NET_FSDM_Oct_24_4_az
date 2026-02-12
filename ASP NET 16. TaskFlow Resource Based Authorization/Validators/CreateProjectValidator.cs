using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Project_DTOs;
using FluentValidation;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");
    }
}
