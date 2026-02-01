using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Application.Persistence;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Algora.Application.Features.Sessions;

public record GetMySessionsQuery(DateTime? From, DateTime? To) : IRequest<SessionsListResponse>;

public record SessionsListResponse(List<SessionResponse> Sessions);

public class GetMySessionsHandler : IRequestHandler<GetMySessionsQuery, SessionsListResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMySessionsHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SessionsListResponse> Handle(GetMySessionsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Get camps user is enrolled in
        var userCamps = await _context.UserRoles
            .Where(r => r.UserId == currentUserId && r.CampId.HasValue)
            .Select(r => r.CampId!.Value)
            .ToListAsync(cancellationToken);

        var query = _context.Sessions.Where(s => userCamps.Contains(s.CampId));

        if (request.From.HasValue)
            query = query.Where(s => s.StartTime >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(s => s.EndTime <= request.To.Value);

        var sessions = await query
            .OrderBy(s => s.StartTime)
            .Select(s => new SessionResponse(s.Id, s.Title, s.SessionType, s.StartTime, s.EndTime))
            .ToListAsync(cancellationToken);

        return new SessionsListResponse(sessions);
    }
}







