using Algora.Domain.Enums;
using Algora.Application.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Leaderboard;

public record GetCampLeaderboardQuery(Guid CampId, Role? FilterByRole, int Top) : IRequest<LeaderboardResponse>;

public record LeaderboardResponse(List<LeaderboardEntry> Entries);
public record LeaderboardEntry(Guid UserId, string Name, int TotalPoints, int SolvedProblems, int Rank);

public class GetCampLeaderboardValidator : AbstractValidator<GetCampLeaderboardQuery>
{
    public GetCampLeaderboardValidator()
    {
        RuleFor(x => x.CampId).NotEmpty();
        RuleFor(x => x.Top).GreaterThan(0).LessThanOrEqualTo(100);
    }
}

public class GetCampLeaderboardHandler : IRequestHandler<GetCampLeaderboardQuery, LeaderboardResponse>
{
    private readonly AlgoraDbContext _context;

    public GetCampLeaderboardHandler(AlgoraDbContext context)
    {
        _context = context;
    }

    public async Task<LeaderboardResponse> Handle(GetCampLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var query = _context.UserCampPoints
            .Where(ucp => ucp.CampId == request.CampId)
            .AsQueryable();

        // Filter by role if specified
        if (request.FilterByRole.HasValue)
        {
            var userIdsWithRole = await _context.UserRoles
                .Where(r => r.Role == request.FilterByRole.Value && r.CampId == request.CampId)
                .Select(r => r.UserId)
                .ToListAsync(cancellationToken);
            
            query = query.Where(ucp => userIdsWithRole.Contains(ucp.UserId));
        }

        var entries = await query
            .OrderByDescending(ucp => ucp.TotalPoints)
            .Take(request.Top)
            .Select(ucp => new
            {
                ucp.UserId,
                ucp.User.FirstName,
                ucp.User.LastName,
                ucp.TotalPoints,
                ucp.SolvedProblemsCount
            })
            .ToListAsync(cancellationToken);

        var leaderboard = entries.Select((e, index) => new LeaderboardEntry(
            e.UserId,
            $"{e.FirstName} {e.LastName}",
            e.TotalPoints,
            e.SolvedProblemsCount,
            index + 1
        )).ToList();

        return new LeaderboardResponse(leaderboard);
    }
}





