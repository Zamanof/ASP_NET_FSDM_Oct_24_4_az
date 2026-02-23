using TaskFlow.Application.DTOs;
using FluentValidation;

namespace TaskFlow.Application.Validators;

public class CreateProjectValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectValidator() => RuleFor(x => x.Name).NotEmpty().WithMessage("Project Name is required").MinimumLength(3).WithMessage("Project Name must be at least 3 characters");
}
