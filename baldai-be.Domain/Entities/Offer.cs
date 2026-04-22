using baldai_be.Domain.Enums;

namespace baldai_be.Domain.Entities;

public class Offer
{
    public Guid Id { get; set; }
    public Guid ChatThreadId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal Amount { get; set; }
    public OfferStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public ChatThread ChatThread { get; set; } = null!;
    public User Buyer { get; set; } = null!;
}
