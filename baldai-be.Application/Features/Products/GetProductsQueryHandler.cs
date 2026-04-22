using baldai_be.Application.Data;
using baldai_be.Application.DTOs;
using baldai_be.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baldai_be.Application.Features.Products;

public record GetProductsQuery(int Page, int PageSize, string? Search, decimal? MaxPrice) : IRequest<PagedResult<ProductSummaryDto>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductSummaryDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.Where(p => p.Status == ProductStatus.Active);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(p => p.Title.Contains(request.Search) || p.Title.Contains(request.Search));
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductSummaryDto(
                p.Id,
                p.Title,
                p.Price,
                string.IsNullOrEmpty(p.ImagePaths) ? "" : p.ImagePaths.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
                p.Status.ToString()
            ))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductSummaryDto>(items, totalCount);
    }
}
