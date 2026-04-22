namespace baldai_be.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChatThreadId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    
    // Optional reference if this message is an offer
    public Guid? OfferId { get; set; }

    public ChatThread ChatThread { get; set; } = null!;
    public User Sender { get; set; } = null!;
    public Offer? Offer { get; set; }
}
