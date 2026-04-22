using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Users;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IApplicationDbContext _context;

    public GetUserProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return new UserProfileDto(
            user.Id,
            user.Email,
            user.Role.ToString(),
            user.Rating,
            user.WalletBalance,
            user.JoinedDate
        );
    }
}
