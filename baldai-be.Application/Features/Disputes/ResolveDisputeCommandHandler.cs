using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Disputes;

public record ResolveDisputeCommand(Guid OrderId, ResolveDisputeDto Dto) : IRequest<string>;

public class ResolveDisputeCommandHandler : IRequestHandler<ResolveDisputeCommand, string>
{
    private readonly IApplicationDbContext _context;

    public ResolveDisputeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(ResolveDisputeCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .ThenInclude(p => p.Seller)
            .Include(t => t.Buyer)
            .SingleOrDefaultAsync(t => t.Id == request.OrderId, cancellationToken);

        if (transaction == null || transaction.Status != TransactionStatus.Disputed)
            throw new InvalidOperationException("Order not found or not disputed");

        transaction.DisputeResolutionNotes = request.Dto.Notes;

        if (request.Dto.Resolution.Equals("Refund", StringComparison.OrdinalIgnoreCase))
        {
            // Refund Buyer
            transaction.Buyer.WalletBalance += transaction.Product.Price;
            transaction.Status = TransactionStatus.Completed; // Or create a new enum like Refunded / Cancelled
            // Reset product to active maybe, or just keep it reserved/cancelled. The spec says "Refund Buyer... Changes Order state to Completed or Refunded."
            // Let's set Product to Active so it can be sold again.
            transaction.Product.Status = ProductStatus.Active;
        }
        else if (request.Dto.Resolution.Equals("Payout", StringComparison.OrdinalIgnoreCase))
        {
            // Payout Seller
            decimal commissionRatio = 0.10m;
            decimal payoutAmount = transaction.Product.Price * (1 - commissionRatio);
            transaction.Product.Seller.WalletBalance += payoutAmount;
            
            transaction.Status = TransactionStatus.Completed;
            transaction.Product.Status = ProductStatus.Sold;
        }
        else 
        {
            throw new ArgumentException("Resolution must be Refund or Payout");
        }

        await _context.SaveChangesAsync(cancellationToken);

        return transaction.Status.ToString();
    }
}
