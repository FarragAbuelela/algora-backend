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

namespace Algora.Application.Features.Sessions;

public record CreateSessionCommand(
    Guid CampId,
    string Title,
    string Description,
    SessionType SessionType,
    DateTime StartTime,
    DateTime EndTime,
    string? Location
) : IRequest<SessionResponse>;

public record SessionResponse(Guid Id, string Title, SessionType Type, DateTime StartTime, DateTime EndTime);

public class CreateSessionValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionValidator()
    {
        RuleFor(x => x.CampId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.StartTime).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
    }
}

public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, SessionResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationService;

    public CreateSessionHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor, INotificationService notificationService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
    }

    public async Task<SessionResponse> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        // Check if user is Community Leader or Instructor for this camp
        var hasPermission = await _context.UserRoles.AnyAsync(r =>
            r.UserId == currentUserId && (r.Role == Role.CommunityLeader || (r.Role == Role.Instructor && r.CampId == request.CampId)),
            cancellationToken);

        if (!hasPermission)
            throw new UnauthorizedAccessException("Only community leaders and instructors can create sessions");

        var session = new Session
        {
            Id = Guid.NewGuid(),
            CampId = request.CampId,
            Title = request.Title,
            Description = request.Description,
            SessionType = request.SessionType,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Location = request.Location,
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        // Notify all students in camp
        var campStudents = await _context.UserRoles
            .Where(r => r.Role == Role.Student && r.CampId == request.CampId)
            .Select(r => r.UserId)
            .ToListAsync(cancellationToken);

        foreach (var studentId in campStudents)
        {
            await _notificationService.CreateNotification(
                studentId,
                "New Session Created",
                $"{request.Title} on {request.StartTime:MMM dd, HH:mm}",
                NotificationType.SessionCreated,
                session.Id
            );
        }

        return new SessionResponse(session.Id, session.Title, session.SessionType, session.StartTime, session.EndTime);
    }
}







