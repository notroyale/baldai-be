using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Orders;

public record MarkShippedCommand(Guid OrderId, Guid SellerId, ShippingUpdateDto Dto) : IRequest;

public class MarkShippedCommandHandler : IRequestHandler<MarkShippedCommand>
{
    private readonly IApplicationDbContext _context;

    public MarkShippedCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(MarkShippedCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .SingleOrDefaultAsync(t => t.Id == request.OrderId, cancellationToken);

        if (transaction == null || transaction.Product.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Not authorized or not found");

        if (transaction.Status != TransactionStatus.Authorized)
            throw new InvalidOperationException("Can only ship authorized orders");

        transaction.Status = TransactionStatus.Shipped;
        transaction.TrackingNumber = request.Dto.TrackingNumber;
        transaction.Carrier = request.Dto.Carrier;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
