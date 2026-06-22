using baldai_be.Domain.Enums;

namespace baldai_be.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public ProductStatus Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dimensions Dimensions { get; set; } = new Dimensions();
    public decimal Price { get; set; }
    public string ImagePaths { get; set; } = string.Empty; // JSON/CSV list of local paths
    public ShippingOption Shipping { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsNew { get; set; }
    public byte[] RowVersion { get; set; } = null!;
    public string? RejectReason { get; set; }

    // Navigation property
    public User Seller { get; set; } = null!;
}
