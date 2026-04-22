using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Products;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailDto>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Seller)
            .SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            throw new KeyNotFoundException("Product not found");

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
    }
}
