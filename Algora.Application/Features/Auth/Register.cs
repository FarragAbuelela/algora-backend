using Algora.Application.Common.Extensions;
using Algora.Application.Common.Interfaces;
using Algora.Domain.Entities;
using Algora.Domain.Enums;
using Algora.Application.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Algora.Application.Features.Auth;

public record RegisterCommand(
    string SCNumber,
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<RegisterResponse>;

public record RegisterResponse(
    Guid UserId,
    string SCNumber,
    string Email,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.SCNumber).MustBeSCNumber();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}

public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IJwtService _jwtService;

    public RegisterHandler(AlgoraDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.SCNumber == request.SCNumber, cancellationToken))
            throw new InvalidOperationException("User with this SCNumber already exists");

        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            throw new InvalidOperationException("User with this email already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            SCNumber = request.SCNumber,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            RefreshToken = _jwtService.GenerateRefreshToken(),
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        var studentRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Role = Role.Student,
            AssignedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.UserRoles.Add(studentRole);
        await _context.SaveChangesAsync(cancellationToken);

        var roles = await _context.UserRoles.Where(r => r.UserId == user.Id).ToListAsync(cancellationToken);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);

        return new RegisterResponse(user.Id, user.SCNumber, user.Email, accessToken, user.RefreshToken!, 900);
    }
}





