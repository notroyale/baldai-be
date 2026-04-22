using baldai_be.Application.Data;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Chats;

public record AcceptOfferCommand(Guid OfferId, Guid SellerId) : IRequest;

public class AcceptOfferCommandHandler : IRequestHandler<AcceptOfferCommand>
{
    private readonly IApplicationDbContext _context;

    public AcceptOfferCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AcceptOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers
            .Include(o => o.ChatThread)
            .SingleOrDefaultAsync(o => o.Id == request.OfferId, cancellationToken);

        if (offer == null) throw new KeyNotFoundException("Offer not found");

        if (offer.ChatThread.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Only the seller can accept the offer");

        if (offer.Status != OfferStatus.Pending || offer.ExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("Offer is not valid or has expired");

        offer.Status = OfferStatus.Accepted;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
