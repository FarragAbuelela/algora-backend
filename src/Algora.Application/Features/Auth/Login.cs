using Algora.Application.Common.Interfaces;
using Algora.Domain.Entities;
using Algora.Application.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Auth;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public record LoginResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IJwtService _jwtService;

    public LoginHandler(AlgoraDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _context.UserRoles.Where(r => r.UserId == user.Id).ToListAsync(cancellationToken);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);

        return new LoginResponse(user.Id, accessToken, user.RefreshToken!, 900);
    }
}





