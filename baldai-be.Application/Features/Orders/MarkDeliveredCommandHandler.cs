using baldai_be.Application.Data;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Orders;

public record MarkDeliveredCommand(Guid OrderId) : IRequest<DateTimeOffset>;

public class MarkDeliveredCommandHandler : IRequestHandler<MarkDeliveredCommand, DateTimeOffset>
{
    private readonly IApplicationDbContext _context;

    public MarkDeliveredCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DateTimeOffset> Handle(MarkDeliveredCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions.SingleOrDefaultAsync(t => t.Id == request.OrderId, cancellationToken);
        if (transaction == null || transaction.Status != TransactionStatus.Shipped)
            throw new InvalidOperationException("Order not found or not shipped.");

        transaction.Status = TransactionStatus.Delivered;
        transaction.EscrowReleaseDate = DateTime.UtcNow.AddHours(48);

        await _context.SaveChangesAsync(cancellationToken);

        return transaction.EscrowReleaseDate.Value;
    }
}
