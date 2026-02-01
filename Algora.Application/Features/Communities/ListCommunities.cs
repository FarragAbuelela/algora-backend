using Algora.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Communities;

public record ListCommunitiesQuery : IRequest<CommunitiesListResponse>;
public record CommunitiesListResponse(List<CommunityResponse> Communities);

public class ListCommunitiesHandler : IRequestHandler<ListCommunitiesQuery, CommunitiesListResponse>
{
    private readonly AlgoraDbContext _context;

    public ListCommunitiesHandler(AlgoraDbContext context)
    {
        _context = context;
    }

    public async Task<CommunitiesListResponse> Handle(ListCommunitiesQuery request, CancellationToken cancellationToken)
    {
        var communities = await _context.Communities
            .Select(c => new CommunityResponse(c.Id, c.Name, c.Description, c.CreatedAt))
            .ToListAsync(cancellationToken);

        return new CommunitiesListResponse(communities);
    }
}





