using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.DTOs.Project_DTOs;
using FluentValidation;

namespace ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Validators;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Project Name is required")
           .MinimumLength(3).WithMessage("Project Name must be at least 3 characters long");
    }
}
