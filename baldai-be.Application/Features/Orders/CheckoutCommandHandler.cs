using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Entities;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Orders;

public record CheckoutCommand(Guid BuyerId, CheckoutDto Dto) : IRequest<TransactionSummaryDto>;

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, TransactionSummaryDto>
{
    private readonly IApplicationDbContext _context;

    public CheckoutCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionSummaryDto> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == request.Dto.ProductId, cancellationToken);
        if (product == null || product.Status != ProductStatus.Active)
            throw new InvalidOperationException("Product is not active or does not exist.");

        var buyer = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.BuyerId, cancellationToken);
        if (buyer == null)
            throw new UnauthorizedAccessException("Buyer not found.");

        decimal finalPrice = product.Price;
        if (request.Dto.OfferId.HasValue)
        {
            var offer = await _context.Offers.SingleOrDefaultAsync(o => o.Id == request.Dto.OfferId.Value, cancellationToken);
            if (offer == null || offer.BuyerId != buyer.Id || offer.Status != baldai_be.Domain.Enums.OfferStatus.Accepted)
                throw new InvalidOperationException("Invalid or not accepted offer");
                
            finalPrice = offer.Amount;
        }

        if (buyer.WalletBalance < finalPrice)
            throw new InvalidOperationException("Insufficient funds.");

        // Deduct wallet balance
        buyer.WalletBalance -= finalPrice;

        // Change status to Reserved
        product.Status = ProductStatus.Reserved;

        // Create transaction
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            BuyerId = buyer.Id,
            Status = TransactionStatus.Authorized
        };

        _context.Transactions.Add(transaction);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("This product was just purchased by someone else.");
        }

        return new TransactionSummaryDto(transaction.Id, transaction.Status.ToString());
    }
}
