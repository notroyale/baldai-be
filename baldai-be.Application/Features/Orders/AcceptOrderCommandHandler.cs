using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Orders;

public record AcceptOrderCommand(Guid OrderId, Guid BuyerId) : IRequest;

public class AcceptOrderCommandHandler : IRequestHandler<AcceptOrderCommand>
{
    private readonly IApplicationDbContext _context;

    public AcceptOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AcceptOrderCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .ThenInclude(p => p.Seller)
            .SingleOrDefaultAsync(t => t.Id == request.OrderId, cancellationToken);

        if (transaction == null || transaction.BuyerId != request.BuyerId)
            throw new UnauthorizedAccessException("Not authorized or order not found");

        if (transaction.Status != TransactionStatus.Delivered)
            throw new InvalidOperationException("Can only accept delivered orders");

        transaction.Status = TransactionStatus.Completed;

        // Perform payout
        decimal commissionRatio = 0.10m; // 10%
        decimal payoutAmount = transaction.Product.Price * (1 - commissionRatio);

        transaction.Product.Seller.WalletBalance += payoutAmount;
        // The remaining 10% stays with the platform.

        // Also note: the product status could be set to Sold
        transaction.Product.Status = ProductStatus.Sold;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
