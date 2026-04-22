using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Chats;

public record GetInboxQuery(Guid UserId) : IRequest<List<ChatThreadDto>>;

public class GetInboxQueryHandler : IRequestHandler<GetInboxQuery, List<ChatThreadDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInboxQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChatThreadDto>> Handle(GetInboxQuery request, CancellationToken cancellationToken)
    {
        var threads = await _context.ChatThreads
            .Include(t => t.Product)
            .Include(t => t.Buyer)
            .Include(t => t.Seller)
            .Include(t => t.Messages)
            .ThenInclude(m => m.Offer)
            .Where(t => t.BuyerId == request.UserId || t.SellerId == request.UserId)
            .ToListAsync(cancellationToken);

        return threads.Select(t => {
            var lastMsg = t.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
            MessageDto? lastMsgDto = null;
            if (lastMsg != null)
            {
                OfferDto? offerDto = null;
                if (lastMsg.Offer != null)
                {
                    offerDto = new OfferDto(lastMsg.Offer.Id, lastMsg.Offer.Amount, lastMsg.Offer.Status.ToString(), lastMsg.Offer.CreatedAt, lastMsg.Offer.ExpiresAt);
                }
                lastMsgDto = new MessageDto(lastMsg.Id, lastMsg.SenderId, lastMsg.Content, lastMsg.SentAt, offerDto);
            }

            return new ChatThreadDto(
                t.Id, 
                t.ProductId, 
                t.Product.Title, 
                t.BuyerId, 
                t.Buyer.FirstName + " " + t.Buyer.LastName, 
                t.SellerId, 
                t.Seller.FirstName + " " + t.Seller.LastName, 
                lastMsgDto
            );
        }).ToList();
    }
}
