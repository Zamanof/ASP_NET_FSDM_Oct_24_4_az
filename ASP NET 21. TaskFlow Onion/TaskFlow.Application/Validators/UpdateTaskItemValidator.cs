using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;
using FluentValidation;
using TaskStatus = TaskFlow.Domain.Entities.TaskStatus;

namespace TaskFlow.Application.Validators;

public class UpdateTaskItemValidator : AbstractValidator<UpdateTaskItemRequest>
{
    public UpdateTaskItemValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("TaskItem Title is required").MinimumLength(3).WithMessage("TaskItem Title must be at least 3 characters");
        RuleFor(x => x.Priority).Must(p => new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High }.Contains(p)).WithMessage("Priority must be one of: Low, Medium, High");
        RuleFor(x => x.Status).Must(s => new[] { TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Done }.Contains(s)).WithMessage("Status must be one of: ToDo, InProgress, Done");
    }
}
