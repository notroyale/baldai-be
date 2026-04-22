using baldai_be.Application.Data;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Moderation;

public record ApproveProductCommand(Guid ProductId) : IRequest;

public class ApproveProductCommandHandler : IRequestHandler<ApproveProductCommand>
{
    private readonly IApplicationDbContext _context;

    public ApproveProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ApproveProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException("Product not found");

        product.Status = ProductStatus.Active;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
