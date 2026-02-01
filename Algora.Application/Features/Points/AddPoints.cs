using System.Security.Claims;
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

namespace Algora.Application.Features.Points;

public record AddPointsCommand(
    Guid TargetUserId,
    Guid CampId,
    int Points,
    string Reason
) : IRequest<PointsResponse>;

public record PointsResponse(Guid UserId, Guid CampId, int TotalPoints, int SolvedProblemsCount);

public class AddPointsValidator : AbstractValidator<AddPointsCommand>
{
    public AddPointsValidator()
    {
        RuleFor(x => x.TargetUserId).NotEmpty();
        RuleFor(x => x.CampId).NotEmpty();
        RuleFor(x => x.Points).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty();
    }
}

public class AddPointsHandler : IRequestHandler<AddPointsCommand, PointsResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddPointsHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PointsResponse> Handle(AddPointsCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var currentUserRoles = await _context.UserRoles.Where(r => r.UserId == currentUserId).ToListAsync(cancellationToken);

        // Check authorization: Admin, Instructor in camp, or Mentor assigned to student
        bool isAdmin = currentUserRoles.Any(r => r.Role == Role.Admin);
        bool isInstructor = currentUserRoles.Any(r => r.Role == Role.Instructor && r.CampId == request.CampId);
        bool isMentor = await _context.MentorAssignments.AnyAsync(m => 
            m.MentorId == currentUserId && m.StudentId == request.TargetUserId && m.IsActive && (!m.CampId.HasValue || m.CampId == request.CampId),
            cancellationToken);

        if (!isAdmin && !isInstructor && !isMentor)
            throw new UnauthorizedAccessException("Insufficient permissions to add points");

        var userPoints = await _context.UserCampPoints
            .FirstOrDefaultAsync(ucp => ucp.UserId == request.TargetUserId && ucp.CampId == request.CampId, cancellationToken);

        if (userPoints == null)
        {
            userPoints = new UserCampPoints
            {
                Id = Guid.NewGuid(),
                UserId = request.TargetUserId,
                CampId = request.CampId,
                TotalPoints = 0,
                SolvedProblemsCount = 0,
                LastUpdated = DateTime.UtcNow
            };
            _context.UserCampPoints.Add(userPoints);
        }

        userPoints.TotalPoints += request.Points;
        userPoints.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return new PointsResponse(userPoints.UserId, userPoints.CampId, userPoints.TotalPoints, userPoints.SolvedProblemsCount);
    }
}







