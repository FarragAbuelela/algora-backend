using Algora.Application.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Points;

public record GetUserPointsQuery(Guid UserId, Guid CampId) : IRequest<PointsResponse>;

public class GetUserPointsValidator : AbstractValidator<GetUserPointsQuery>
{
    public GetUserPointsValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CampId).NotEmpty();
    }
}

public class GetUserPointsHandler : IRequestHandler<GetUserPointsQuery, PointsResponse>
{
    private readonly AlgoraDbContext _context;

    public GetUserPointsHandler(AlgoraDbContext context)
    {
        _context = context;
    }

    public async Task<PointsResponse> Handle(GetUserPointsQuery request, CancellationToken cancellationToken)
    {
        var userPoints = await _context.UserCampPoints
            .FirstOrDefaultAsync(ucp => ucp.UserId == request.UserId && ucp.CampId == request.CampId, cancellationToken);

        if (userPoints == null)
        {
            return new PointsResponse(request.UserId, request.CampId, 0, 0);
        }

        return new PointsResponse(userPoints.UserId, userPoints.CampId, userPoints.TotalPoints, userPoints.SolvedProblemsCount);
    }
}
