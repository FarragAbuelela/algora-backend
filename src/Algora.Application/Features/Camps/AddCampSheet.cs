using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Domain.Entities;
using Algora.Domain.Enums;
using Algora.Application.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Camps;

public record AddCampSheetCommand(Guid CampId, string Title, string Description, string? Url) : IRequest<CampSheetResponse>;
public record CampSheetResponse(Guid Id, string Title, string Url);

public class AddCampSheetValidator : AbstractValidator<AddCampSheetCommand>
{
    public AddCampSheetValidator()
    {
        RuleFor(x => x.CampId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
    }
}

public class AddCampSheetHandler : IRequestHandler<AddCampSheetCommand, CampSheetResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddCampSheetHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CampSheetResponse> Handle(AddCampSheetCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        // Check if user has permission (Instructor, Community Leader, or Admin)
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

        var sheet = new CampSheet
        {
            Id = Guid.NewGuid(),
            CampId = request.CampId,
            Name = request.Title,
            Url = request.Url,
            Description = request.Description,
            Order = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.CampSheets.Add(sheet);
        await _context.SaveChangesAsync(cancellationToken);

        return new CampSheetResponse(sheet.Id, sheet.Name, sheet.Url ?? "");
    }
}
