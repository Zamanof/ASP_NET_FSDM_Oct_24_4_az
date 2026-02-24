using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_21._TaskFlow_CQRS.Domain;
using FluentValidation;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Validators;

public class CreateTaskItemValidator : AbstractValidator<CreateTaskItemRequest>
{
    public CreateTaskItemValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("TaskItem Title is required").MinimumLength(3).WithMessage("TaskItem Title must be at least 3 characters");
        RuleFor(x => x.ProjectId).NotEmpty().WithMessage("ProjectId is required").GreaterThan(0).WithMessage("ProjectId must be greater than 0");
        RuleFor(x => x.Priority).Must(p => new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High }.Contains(p)).WithMessage("Priority must be one of: Low, Medium, High");
    }
}
