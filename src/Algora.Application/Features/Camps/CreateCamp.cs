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

namespace Algora.Application.Features.Camps;

public record CreateCampCommand(
    Guid CommunityId,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime? EndDate
) : IRequest<CampResponse>;

public record CampResponse(Guid Id, string Name, DateTime StartDate, DateTime? EndDate);

public class CreateCampValidator : AbstractValidator<CreateCampCommand>
{
    public CreateCampValidator()
    {
        RuleFor(x => x.CommunityId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
    }
}

public class CreateCampHandler : IRequestHandler<CreateCampCommand, CampResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCampHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CampResponse> Handle(CreateCampCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        // Check if user is Community Leader for this community or Admin
        var hasPermission = await _context.UserRoles.AnyAsync(r =>
            r.UserId == currentUserId && (r.Role == Role.Admin || (r.Role == Role.CommunityLeader && r.CommunityId == request.CommunityId)),
            cancellationToken);

        if (!hasPermission)
            throw new UnauthorizedAccessException("Only community leaders and admins can create camps");

        var camp = new Camp
        {
            Id = Guid.NewGuid(),
            CommunityId = request.CommunityId,
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Camps.Add(camp);
        await _context.SaveChangesAsync(cancellationToken);

        return new CampResponse(camp.Id, camp.Name, camp.StartDate, camp.EndDate);
    }
}







