using baldai_be.Domain.Enums;

namespace baldai_be.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid BuyerId { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime? EscrowReleaseDate { get; set; }
    
    // Logistics
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public string? PickupCode { get; set; }

    // Disputes
    public string? DisputeReason { get; set; }
    public string? DisputeEvidenceUrls { get; set; }
    public string? DisputeResolutionNotes { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public User Buyer { get; set; } = null!;
}
