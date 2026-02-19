using ASP_NET_18._TaskFlow_Refactoring.DTOs;
using FluentValidation;

namespace ASP_NET_18._TaskFlow_Refactoring.Validators;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Project Name is required")
           .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");
    }
}
