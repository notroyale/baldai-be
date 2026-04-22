using baldai_be.Application.Data;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Chats;

public record DeclineOfferCommand(Guid OfferId, Guid SellerId) : IRequest;

public class DeclineOfferCommandHandler : IRequestHandler<DeclineOfferCommand>
{
    private readonly IApplicationDbContext _context;

    public DeclineOfferCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeclineOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers
            .Include(o => o.ChatThread)
            .SingleOrDefaultAsync(o => o.Id == request.OfferId, cancellationToken);

        if (offer == null) throw new KeyNotFoundException("Offer not found");

        if (offer.ChatThread.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Only the seller can decline the offer");

        if (offer.Status != OfferStatus.Pending) // Allow declining expired offers technically or restrict it
            throw new InvalidOperationException("Offer is not in pending state");

        offer.Status = OfferStatus.Rejected;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
