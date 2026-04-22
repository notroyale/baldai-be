using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Chats;

public record SendMessageCommand(Guid UserId, CreateMessageDto Dto) : IRequest<MessageDto>;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IApplicationDbContext _context;

    public SendMessageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == request.Dto.ProductId, cancellationToken);
        if (product == null) throw new KeyNotFoundException("Product not found");

        var thread = await _context.ChatThreads
            .FirstOrDefaultAsync(t => t.ProductId == request.Dto.ProductId && 
                (t.BuyerId == request.UserId || t.SellerId == request.UserId), cancellationToken);

        if (thread == null)
        {
            // Auto create thread if it's the buyer initiating
            if (product.SellerId == request.UserId)
                throw new InvalidOperationException("Sellers cannot initiate a chat with themselves without a buyer context.");

            thread = new ChatThread
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                BuyerId = request.UserId,
                SellerId = product.SellerId
            };
            _context.ChatThreads.Add(thread);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatThreadId = thread.Id,
            SenderId = request.UserId,
            Content = request.Dto.Content,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        return new MessageDto(message.Id, message.SenderId, message.Content, message.SentAt, null);
    }
}
