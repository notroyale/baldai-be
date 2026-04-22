using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Moderation;

public record RejectProductCommand(Guid ProductId, RejectDto Dto) : IRequest;

public class RejectProductCommandHandler : IRequestHandler<RejectProductCommand>
{
    private readonly IApplicationDbContext _context;

    public RejectProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RejectProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException("Product not found");

        product.Status = ProductStatus.Rejected;
        product.RejectReason = request.Dto.Reason;
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}
