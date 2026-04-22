using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Entities;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Chats;

public record CreateOfferCommand(Guid BuyerId, CreateOfferDto Dto) : IRequest<MessageDto>;

public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, MessageDto>
{
    private readonly IApplicationDbContext _context;

    public CreateOfferCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MessageDto> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == request.Dto.ProductId, cancellationToken);
        if (product == null) throw new KeyNotFoundException("Product not found");

        if (product.SellerId == request.BuyerId)
            throw new InvalidOperationException("You cannot make an offer on your own product.");

        // Do we have a thread?
        var thread = await _context.ChatThreads
            .FirstOrDefaultAsync(t => t.ProductId == product.Id && t.BuyerId == request.BuyerId, cancellationToken);

        if (thread == null)
        {
            thread = new ChatThread
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                BuyerId = request.BuyerId,
                SellerId = product.SellerId
            };
            _context.ChatThreads.Add(thread);
        }

        // Check if there's already an active offer
        var existingActiveOffer = await _context.Offers
            .AnyAsync(o => o.ChatThreadId == thread.Id && o.Status == OfferStatus.Pending && o.ExpiresAt > DateTime.UtcNow, cancellationToken);
        
        if (existingActiveOffer)
            throw new InvalidOperationException("You already have an active offer. Please wait for the seller to respond.");

        var offer = new Offer
        {
            Id = Guid.NewGuid(),
            ChatThreadId = thread.Id,
            BuyerId = request.BuyerId,
            Amount = request.Dto.Amount,
            Status = OfferStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24) // 24 lock frame standard
        };
        _context.Offers.Add(offer);

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatThreadId = thread.Id,
            SenderId = request.BuyerId,
            Content = request.Dto.MessageContent,
            SentAt = DateTime.UtcNow,
            OfferId = offer.Id,
            Offer = offer
        };
        _context.Messages.Add(message);

        await _context.SaveChangesAsync(cancellationToken);

        var offerDto = new OfferDto(offer.Id, offer.Amount, offer.Status.ToString(), offer.CreatedAt, offer.ExpiresAt);
        return new MessageDto(message.Id, message.SenderId, message.Content, message.SentAt, offerDto);
    }
}
