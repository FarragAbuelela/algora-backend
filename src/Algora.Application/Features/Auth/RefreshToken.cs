using Algora.Application.Common.Interfaces;
using Algora.Application.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Auth;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<RefreshTokenResponse>;

public record RefreshTokenResponse(string AccessToken, string RefreshToken);

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshTokenHandler(AlgoraDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            throw new UnauthorizedAccessException("Invalid access token");

        var userId = Guid.Parse(principal.Identity!.Name!);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var roles = await _context.UserRoles.Where(r => r.UserId == user.Id).ToListAsync(cancellationToken);
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(newAccessToken, newRefreshToken);
    }
}
