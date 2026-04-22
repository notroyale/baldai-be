using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Logistics;

public record VerifyPickupCommand(Guid OrderId, Guid SellerId, VerifyPickupDto Dto) : IRequest;

public class VerifyPickupCommandHandler : IRequestHandler<VerifyPickupCommand>
{
    private readonly IApplicationDbContext _context;

    public VerifyPickupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(VerifyPickupCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .SingleOrDefaultAsync(t => t.Id == request.OrderId, cancellationToken);
            
        if (transaction == null || transaction.Product.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Not authorized or order not found");

        if (transaction.Status != TransactionStatus.Authorized)
            throw new InvalidOperationException("Order not in authorized state");

        if (string.IsNullOrEmpty(transaction.PickupCode) || transaction.PickupCode != request.Dto.Code)
            throw new InvalidOperationException("Invalid pickup code");

        // Instantly switch to Delivery
        transaction.Status = TransactionStatus.Delivered;
        transaction.EscrowReleaseDate = DateTime.UtcNow.AddHours(48);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
