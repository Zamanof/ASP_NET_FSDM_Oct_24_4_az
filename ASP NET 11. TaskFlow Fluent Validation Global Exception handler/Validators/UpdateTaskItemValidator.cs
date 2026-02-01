using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.DTOs.Task_Items_DTOs;
using ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Models;
using FluentValidation;
using TaskStatus = ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Models.TaskStatus;

namespace ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Validators;

public class UpdateTaskItemValidator : AbstractValidator<UpdateTaskItemRequest>
{
    public UpdateTaskItemValidator()
    {
        RuleFor(x => x.Title)
           .NotEmpty().WithMessage("TaskItem Title is required")
           .MinimumLength(3).WithMessage("TaskItem Title must be at least 3 characters long");

        RuleFor(x => x.Priority)
            .Must(p => new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High }.Contains(p))
            .WithMessage("TaskItem Prioity must be one of: 0(Low), 1(Medium), 2(High)");

        RuleFor(x => x.Status)
            .Must(s => new[] { TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Done }.Contains(s))
            .WithMessage("TaskItem Status must be one of: 0(ToDo), 1(InProgress), 2(Done)");
    }
}
