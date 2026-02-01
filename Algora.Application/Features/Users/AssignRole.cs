using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Domain.Entities;
using Algora.Domain.Enums;
using Algora.Application.Persistence;
using Algora.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Users;

public record AssignRoleCommand(Guid UserId, Role Role, Guid? CampId, Guid? CommunityId) : IRequest<RoleAssignmentResponse>;
public record RoleAssignmentResponse(Guid RoleId, Guid UserId, Role Role);

public class AssignRoleValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
    }
}

public class AssignRoleHandler : IRequestHandler<AssignRoleCommand, RoleAssignmentResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationService;

    public AssignRoleHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor, INotificationService notificationService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
    }

    public async Task<RoleAssignmentResponse> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var currentUserRoles = await _context.UserRoles.Where(r => r.UserId == currentUserId).ToListAsync(cancellationToken);

        bool isAdmin = currentUserRoles.Any(r => r.Role == Role.Admin);
        bool isCommunityLeader = currentUserRoles.Any(r => r.Role == Role.CommunityLeader && r.CommunityId == request.CommunityId);

        if (!isAdmin && !isCommunityLeader)
            throw new UnauthorizedAccessException("Insufficient permissions");

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Role = request.Role,
            CampId = request.CampId,
            CommunityId = request.CommunityId,
            AssignedAt = DateTime.UtcNow
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateNotification(
            request.UserId,
            "Role Assigned",
            $"You have been assigned the role: {request.Role}",
            NotificationType.RoleAssigned,
            userRole.Id
        );

        return new RoleAssignmentResponse(userRole.Id, userRole.UserId, userRole.Role);
    }
}
