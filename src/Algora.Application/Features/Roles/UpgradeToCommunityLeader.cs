using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Algora.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Algora.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Algora.Application.Persistence;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Algora.Application.Features.Roles;

public record UpgradeToCommunityLeaderCommand(Guid UserId, Guid CommunityId) : IRequest<RoleAssignmentResponse>;

public record RoleAssignmentResponse(Guid RoleId, Guid UserId, Role Role, DateTime AssignedAt);

public class UpgradeToCommunityLeaderValidator : AbstractValidator<UpgradeToCommunityLeaderCommand>
{
    public UpgradeToCommunityLeaderValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CommunityId).NotEmpty();
    }
}

public class UpgradeToCommunityLeaderHandler : IRequestHandler<UpgradeToCommunityLeaderCommand, RoleAssignmentResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationService;

    public UpgradeToCommunityLeaderHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor, INotificationService notificationService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
    }

    public async Task<RoleAssignmentResponse> Handle(UpgradeToCommunityLeaderCommand request, CancellationToken cancellationToken)
    {
        // Check if current user is Admin
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = await _context.UserRoles.AnyAsync(r => r.UserId == currentUserId && r.Role == Role.Admin, cancellationToken);
        
        if (!isAdmin)
            throw new UnauthorizedAccessException("Only admins can assign community leader role");

        if (!await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken))
            throw new InvalidOperationException("User not found");

        if (!await _context.Communities.AnyAsync(c => c.Id == request.CommunityId, cancellationToken))
            throw new InvalidOperationException("Community not found");

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Role = Role.CommunityLeader,
            CommunityId = request.CommunityId,
            AssignedAt = DateTime.UtcNow
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateNotification(
            request.UserId,
            "Role Assigned",
            "You have been assigned as Community Leader",
            NotificationType.RoleAssigned,
            userRole.Id
        );

        return new RoleAssignmentResponse(userRole.Id, userRole.UserId, userRole.Role, userRole.AssignedAt);
    }
}







