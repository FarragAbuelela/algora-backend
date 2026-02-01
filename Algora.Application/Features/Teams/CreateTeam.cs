using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Algora.Application.Persistence;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Algora.Application.Features.Teams;

public record CreateTeamCommand(Guid CampId, string Name) : IRequest<TeamResponse>;
public record TeamResponse(Guid Id, string Name, Guid TeamLeaderId);

public class CreateTeamValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.CampId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class CreateTeamHandler : IRequestHandler<CreateTeamCommand, TeamResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateTeamHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
_httpContextAccessor = httpContextAccessor;
    }

    public async Task<TeamResponse> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var team = new Team
        {
            Id = Guid.NewGuid(),
            CampId = request.CampId,
            Name = request.Name,
            TeamLeaderId = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        var teamMember = new TeamMember
        {
            Id = Guid.NewGuid(),
            TeamId = team.Id,
            UserId = currentUserId,
            JoinedAt = DateTime.UtcNow
        };

        _context.Teams.Add(team);
        _context.TeamMembers.Add(teamMember);
        await _context.SaveChangesAsync(cancellationToken);

        return new TeamResponse(team.Id, team.Name, team.TeamLeaderId);
    }
}







