using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Domain.Entities;
using Algora.Domain.Enums;
using Algora.Application.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Camps;

public record AddProblemCommand(
    Guid CampId,
    string Title,
    string Description,
    int Points,
    string Difficulty,
    string? Url
) : IRequest<ProblemResponse>;

public record ProblemResponse(Guid Id, string Title, int Points, string Difficulty);

public class AddProblemValidator : AbstractValidator<AddProblemCommand>
{
    public AddProblemValidator()
    {
        RuleFor(x => x.CampId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Points).GreaterThan(0);
        RuleFor(x => x.Difficulty).NotEmpty();
    }
}

public class AddProblemHandler : IRequestHandler<AddProblemCommand, ProblemResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddProblemHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ProblemResponse> Handle(AddProblemCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var camp = await _context.Camps.FirstOrDefaultAsync(c => c.Id == request.CampId, cancellationToken);
        if (camp == null)
            throw new InvalidOperationException("Camp not found");

        var hasPermission = await _context.UserRoles.AnyAsync(r =>
            r.UserId == currentUserId && (r.Role == Role.Admin ||
            (r.Role == Role.CommunityLeader && r.CommunityId == camp.CommunityId) ||
            (r.Role == Role.Instructor && r.CampId == request.CampId)),
            cancellationToken);

        if (!hasPermission)
            throw new UnauthorizedAccessException("Insufficient permissions");

        var problem = new Problem
        {
            Id = Guid.NewGuid(),
            CampId = request.CampId,
            Title = request.Title,
            Description = request.Description,
            Points = request.Points,
            Difficulty = request.Difficulty,
            Url = request.Url,
            CreatedAt = DateTime.UtcNow
        };

        _context.Problems.Add(problem);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProblemResponse(problem.Id, problem.Title, problem.Points, problem.Difficulty);
    }
}
