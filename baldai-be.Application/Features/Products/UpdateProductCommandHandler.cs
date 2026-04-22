using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Products;

public record UpdateProductCommand(Guid ProductId, Guid SellerId, ProductCreateDto Dto) : IRequest;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == request.ProductId && p.SellerId == request.SellerId, cancellationToken);
        if (product == null)
            throw new UnauthorizedAccessException("Product not found or not owned by the seller");

        var dto = request.Dto;

        if (!Enum.TryParse<ShippingOption>(dto.ShippingOption, true, out var shippingOption))
            throw new ArgumentException("Invalid shipping option");

        product.Title = dto.Title;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Dimensions = new baldai_be.Domain.Entities.Dimensions { Length = dto.Dimensions.Length, Width = dto.Dimensions.Width, Height = dto.Dimensions.Height };
        product.Shipping = shippingOption;
        product.ImagePaths = string.Join(",", dto.Images);
        
        // "Crucial logic: If listing was Active, update reverts to PendingModeration"
        if (product.Status == ProductStatus.Active)
        {
            product.Status = ProductStatus.PendingModeration;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
