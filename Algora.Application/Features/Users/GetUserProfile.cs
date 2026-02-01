using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Application.Persistence;
using Algora.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Users;

public record GetUserProfileQuery : IRequest<UserProfileResponse>;

public record UserProfileResponse(
    Guid Id,
    string SCNumber,
    string Email,
    string FirstName,
    string LastName,
    List<UserRoleDto> Roles
);

public record UserRoleDto(Role Role, Guid? CampId, Guid? CommunityId);

public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetUserProfileHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var roles = await _context.UserRoles
            .Where(r => r.UserId == currentUserId)
            .Select(r => new UserRoleDto(r.Role, r.CampId, r.CommunityId))
            .ToListAsync(cancellationToken);

        return new UserProfileResponse(user.Id, user.SCNumber, user.Email, user.FirstName, user.LastName, roles);
    }
}
