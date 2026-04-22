using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using baldai_be.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Products;

public record CreateProductCommand(Guid SellerId, ProductCreateDto Dto) : IRequest<Guid>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (!Enum.TryParse<ShippingOption>(dto.ShippingOption, true, out var shippingOption))
        {
            throw new ArgumentException("Invalid shipping option");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            SellerId = request.SellerId,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Dimensions = new Dimensions { Length = dto.Dimensions.Length, Width = dto.Dimensions.Width, Height = dto.Dimensions.Height },
            Shipping = shippingOption,
            ImagePaths = string.Join(",", dto.Images),
            Status = ProductStatus.PendingModeration
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
