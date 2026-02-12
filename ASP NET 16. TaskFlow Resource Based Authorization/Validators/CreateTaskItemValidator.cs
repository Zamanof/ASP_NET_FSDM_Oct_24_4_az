using ASP_NET_16._TaskFlow_Resource_Based_Authorization.DTOs.Task_Items_DTOs;
using ASP_NET_16._TaskFlow_Resource_Based_Authorization.Models;
using FluentValidation;

namespace ASP_NET_16._TaskFlow_Resource_Based_Authorization.Validators;

public class CreateTaskItemValidator : AbstractValidator<CreateTaskItemRequest>
{
    public CreateTaskItemValidator()
    {
        RuleFor(x => x.Title)
           .NotEmpty().WithMessage("TaskItem Title is required")
           .MinimumLength(3).WithMessage("TaskItem Title must be at least 3 characters long");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required")
            .GreaterThan(0).WithMessage("ProjectId must be greater than 0");

        RuleFor(x => x.Priority)
            .Must(p => new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High }.Contains(p))
            .WithMessage("TaskItem Prioity must be one of: 0(Low), 1(Medium), 2(High)");
    }
}
