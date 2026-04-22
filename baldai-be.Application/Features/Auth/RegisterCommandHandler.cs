using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Entities;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Auth;

public record RegisterCommand(RegisterDto Dto) : IRequest<RegisterResponse>;

public record RegisterResponse(Guid UserId, string Email, string Role);

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IApplicationDbContext _context;
    // We can instantiate PasswordHasher directly as it doesn't need DI necessarily for simple usage
    private readonly PasswordHasher<User> _passwordHasher;

    public RegisterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists");
        }

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var parsedRole))
        {
            throw new ArgumentException("Invalid role");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = parsedRole,
            WalletBalance = 0.00m,
            Rating = 0.0m,
            JoinedDate = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterResponse(user.Id, user.Email, user.Role.ToString());
    }
}
