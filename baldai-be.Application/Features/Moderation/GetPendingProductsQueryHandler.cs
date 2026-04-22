using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Moderation;

public record GetPendingProductsQuery() : IRequest<List<ProductDetailDto>>;

public class GetPendingProductsQueryHandler : IRequestHandler<GetPendingProductsQuery, List<ProductDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDetailDto>> Handle(GetPendingProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _context.Products
            .Include(p => p.Seller)
            .Where(p => p.Status == ProductStatus.PendingModeration)
            .ToListAsync(cancellationToken);

        return products.Select(product =>
        {
            var images = string.IsNullOrEmpty(product.ImagePaths)
                ? new List<string>()
                : product.ImagePaths.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            return new ProductDetailDto(
                product.Id,
                product.Title,
                product.Description,
                product.Price,
                new DimensionsDto(product.Dimensions.Length, product.Dimensions.Width, product.Dimensions.Height),
                images,
                product.Shipping.ToString(),
                new SellerDto(product.Seller.Id, product.Seller.FirstName + " " + product.Seller.LastName, product.Seller.Rating),
                product.Status.ToString()
            );
        }).ToList();
    }
}
