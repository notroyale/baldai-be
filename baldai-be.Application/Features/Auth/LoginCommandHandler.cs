using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Auth;

public record LoginCommand(LoginDto Dto) : IRequest<LoginResponse>;

public record LoginResponse(string Token, int ExpiresIn, UserMetadata User);
public record UserMetadata(Guid Id, string Email, string Role);

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtProvider _jwtProvider;
    private readonly PasswordHasher<User> _passwordHasher;

    public LoginCommandHandler(IApplicationDbContext context, IJwtProvider jwtProvider)
    {
        _context = context;
        _jwtProvider = jwtProvider;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Dto.Email, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Dto.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _jwtProvider.Generate(user);
        
        return new LoginResponse(
            Token: token,
            ExpiresIn: 7200, // 2 hours
            User: new UserMetadata(user.Id, user.Email, user.Role.ToString())
        );
    }
}
