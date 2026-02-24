using ASP_NET_21._TaskFlow_CQRS.Application.DTOs;
using ASP_NET_21._TaskFlow_CQRS.Domain;
using FluentValidation;
using TaskStatus = ASP_NET_21._TaskFlow_CQRS.Domain.TaskStatus;

namespace ASP_NET_21._TaskFlow_CQRS.Application.Validators;

public class UpdateTaskItemValidator : AbstractValidator<UpdateTaskItemRequest>
{
    public UpdateTaskItemValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("TaskItem Title is required").MinimumLength(3).WithMessage("TaskItem Title must be at least 3 characters");
        RuleFor(x => x.Priority).Must(p => new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High }.Contains(p)).WithMessage("Priority must be one of: Low, Medium, High");
        RuleFor(x => x.Status).Must(s => new[] { TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Done }.Contains(s)).WithMessage("Status must be one of: ToDo, InProgress, Done");
    }
}
