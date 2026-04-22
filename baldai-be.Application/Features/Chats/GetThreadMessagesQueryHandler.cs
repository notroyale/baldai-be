using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Chats;

public record GetThreadMessagesQuery(Guid ChatThreadId, Guid UserId) : IRequest<List<MessageDto>>;

public class GetThreadMessagesQueryHandler : IRequestHandler<GetThreadMessagesQuery, List<MessageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetThreadMessagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageDto>> Handle(GetThreadMessagesQuery request, CancellationToken cancellationToken)
    {
        var thread = await _context.ChatThreads
            .Include(t => t.Messages)
            .ThenInclude(m => m.Offer)
            .SingleOrDefaultAsync(t => t.Id == request.ChatThreadId, cancellationToken);
            
        if (thread == null) throw new KeyNotFoundException("Chat thread not found.");
        
        if (thread.BuyerId != request.UserId && thread.SellerId != request.UserId)
            throw new UnauthorizedAccessException("You are not part of this chat thread.");

        return thread.Messages.OrderBy(m => m.SentAt).Select(m => {
            OfferDto? offerDto = null;
            if (m.Offer != null)
            {
                offerDto = new OfferDto(m.Offer.Id, m.Offer.Amount, m.Offer.Status.ToString(), m.Offer.CreatedAt, m.Offer.ExpiresAt);
            }
            return new MessageDto(m.Id, m.SenderId, m.Content, m.SentAt, offerDto);
        }).ToList();
    }
}
