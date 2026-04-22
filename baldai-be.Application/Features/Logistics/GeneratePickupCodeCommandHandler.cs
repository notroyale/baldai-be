using baldai_be.Application.Data;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Logistics;

public record GeneratePickupCodeCommand(Guid OrderId, Guid BuyerId) : IRequest<string>;

public class GeneratePickupCodeCommandHandler : IRequestHandler<GeneratePickupCodeCommand, string>
{
    private readonly IApplicationDbContext _context;

    public GeneratePickupCodeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(GeneratePickupCodeCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .SingleOrDefaultAsync(t => t.Id == request.OrderId, cancellationToken);
            
        if (transaction == null || transaction.BuyerId != request.BuyerId)
            throw new UnauthorizedAccessException("Not authorized or order not found");

        if (transaction.Status != TransactionStatus.Authorized)
            throw new InvalidOperationException("Can only generate code for authorized orders");

        if (string.IsNullOrEmpty(transaction.PickupCode))
        {
            var random = new Random();
            transaction.PickupCode = random.Next(100000, 999999).ToString();
            await _context.SaveChangesAsync(cancellationToken);
        }

        return transaction.PickupCode;
    }
}
