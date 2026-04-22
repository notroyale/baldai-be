using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Orders;

public record GetOrderStateQuery(Guid OrderId, Guid UserId) : IRequest<OrderStateDto>;

public class GetOrderStateQueryHandler : IRequestHandler<GetOrderStateQuery, OrderStateDto>
{
    private readonly IApplicationDbContext _context;

    public GetOrderStateQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderStateDto> Handle(GetOrderStateQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .SingleOrDefaultAsync(t => t.Id == request.OrderId, cancellationToken);

        if (transaction == null)
            throw new KeyNotFoundException("Order not found.");

        if (transaction.BuyerId != request.UserId && transaction.Product.SellerId != request.UserId)
            throw new UnauthorizedAccessException("You don't have access to this order.");

        var timeline = new List<OrderStatusTimelineDto>
        {
            new OrderStatusTimelineDto("Authorized", DateTime.UtcNow.ToString("O")) // In reality, we'd pull from audit logs. Mocking timeline for now.
        };

        if (transaction.Status >= TransactionStatus.Shipped)
            timeline.Add(new OrderStatusTimelineDto("Shipped", DateTime.UtcNow.ToString("O")));

        return new OrderStateDto(
            transaction.Id,
            transaction.Status.ToString(),
            transaction.Status == TransactionStatus.Shipped || transaction.Status == TransactionStatus.Delivered || transaction.Status == TransactionStatus.Disputed,
            transaction.Product.Shipping.ToString(),
            timeline
        );
    }
}
