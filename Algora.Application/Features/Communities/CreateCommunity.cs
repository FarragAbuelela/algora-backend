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

namespace Algora.Application.Features.Communities;

public record CreateCommunityCommand(string Name, string Description) : IRequest<CommunityResponse>;
public record CommunityResponse(Guid Id, string Name, string Description, DateTime CreatedAt);

public class CreateCommunityValidator : AbstractValidator<CreateCommunityCommand>
{
    public CreateCommunityValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
    }
}

public class CreateCommunityHandler : IRequestHandler<CreateCommunityCommand, CommunityResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCommunityHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommunityResponse> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = await _context.UserRoles.AnyAsync(r => r.UserId == currentUserId && r.Role == Role.Admin, cancellationToken);
        
        if (!isAdmin)
            throw new UnauthorizedAccessException("Only admins can create communities");

        var community = new Community
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Communities.Add(community);
        await _context.SaveChangesAsync(cancellationToken);

        return new CommunityResponse(community.Id, community.Name, community.Description, community.CreatedAt);
    }
}







