using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Disputes;

public record OpenDisputeCommand(Guid BuyerId, OpenDisputeDto Dto) : IRequest<Guid>;

public class OpenDisputeCommandHandler : IRequestHandler<OpenDisputeCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public OpenDisputeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(OpenDisputeCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .SingleOrDefaultAsync(t => t.Id == request.Dto.OrderId, cancellationToken);

        if (transaction == null || transaction.BuyerId != request.BuyerId)
            throw new UnauthorizedAccessException("Not authorized or order not found");

        if (transaction.Status != TransactionStatus.Delivered)
            throw new InvalidOperationException("Can only dispute delivered items");

        transaction.Status = TransactionStatus.Disputed;
        transaction.DisputeReason = request.Dto.Reason;
        transaction.DisputeEvidenceUrls = string.Join(",", request.Dto.ImageEvidenceUrls);
        
        // Pause timer
        transaction.EscrowReleaseDate = null; 

        await _context.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
