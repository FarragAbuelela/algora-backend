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

namespace Algora.Application.Features.Mentorship;

public record AssignStudentToMentorCommand(Guid MentorId, Guid StudentId, Guid? CampId) : IRequest<MentorAssignmentResponse>;

public record MentorAssignmentResponse(Guid Id, Guid MentorId, Guid StudentId, DateTime AssignedAt);

public class AssignStudentToMentorValidator : AbstractValidator<AssignStudentToMentorCommand>
{
    public AssignStudentToMentorValidator()
    {
        RuleFor(x => x.MentorId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}

public class AssignStudentToMentorHandler : IRequestHandler<AssignStudentToMentorCommand, MentorAssignmentResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationService;

    public AssignStudentToMentorHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor, INotificationService notificationService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
    }

    public async Task<MentorAssignmentResponse> Handle(AssignStudentToMentorCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var currentUserRoles = await _context.UserRoles.Where(r => r.UserId == currentUserId).ToListAsync(cancellationToken);

        // Check authorization: Admin, Community Leader, or Instructor
        bool isAuthorized = currentUserRoles.Any(r => r.Role == Role.Admin || r.Role == Role.CommunityLeader ||
            (r.Role == Role.Instructor && (!request.CampId.HasValue || r.CampId == request.CampId)));

        if (!isAuthorized)
            throw new UnauthorizedAccessException("Insufficient permissions");

        var assignment = new MentorAssignment
        {
            Id = Guid.NewGuid(),
            MentorId = request.MentorId,
            StudentId = request.StudentId,
            CampId = request.CampId,
            AssignedBy = currentUserId,
            AssignedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.MentorAssignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateNotification(request.MentorId, "Student Assigned", "A student has been assigned to you", NotificationType.MentorAssigned, assignment.Id);
        await _notificationService.CreateNotification(request.StudentId, "Mentor Assigned", "You have been assigned a mentor", NotificationType.MentorAssigned, assignment.Id);

        return new MentorAssignmentResponse(assignment.Id, assignment.MentorId, assignment.StudentId, assignment.AssignedAt);
    }
}







