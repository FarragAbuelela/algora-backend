using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Algora.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Algora.Application.Persistence;
using Microsoft.AspNetCore.Http;
using Algora.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TaskStatusEnum = Algora.Domain.Enums.TaskStatus;

namespace Algora.Application.Features.Tasks;

public record CreateTaskCommand(
    string Title,
    string Description,
    Guid StudentId,
    Guid? CampId,
    DateTime? DueDate,
    int RequiredCount
) : IRequest<TaskResponse>;

public record TaskResponse(Guid Id, string Title, TaskStatusEnum Status, DateTime? DueDate, int RequiredCount);

public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.RequiredCount).GreaterThan(0);
    }
}

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationService;

    public CreateTaskHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor, INotificationService notificationService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
    }

    public async Task<TaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Check if current user is assigned as mentor to this student
        var isMentor = await _context.MentorAssignments
            .AnyAsync(m => m.MentorId == currentUserId && m.StudentId == request.StudentId && m.IsActive, cancellationToken);

        if (!isMentor)
            throw new UnauthorizedAccessException("You are not assigned as a mentor to this student");

        var task = new UserTask
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedByMentorId = currentUserId,
            AssignedToStudentId = request.StudentId,
            CampId = request.CampId,
            DueDate = request.DueDate,
            RequiredCount = request.RequiredCount,
            Status = TaskStatusEnum.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserTasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateNotification(
            request.StudentId,
            "New Task Assigned",
            $"You have a new task: {request.Title}",
            NotificationType.TaskAssigned,
            task.Id
        );

        return new TaskResponse(task.Id, task.Title, task.Status, task.DueDate, task.RequiredCount);
    }
}








