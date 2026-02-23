using ASP_NET_20._TaskFlow.BLL.DTOs;
using FluentValidation;

namespace ASP_NET_20._TaskFlow.API.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectValidator() => RuleFor(x => x.Name).NotEmpty().WithMessage("Project Name is required").MinimumLength(3).WithMessage("Project Name must be at least 3 characters");
}
