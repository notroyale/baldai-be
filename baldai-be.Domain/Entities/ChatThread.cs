namespace baldai_be.Domain.Entities;

public class ChatThread
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }

    // Nav
    public Product Product { get; set; } = null!;
    public User Buyer { get; set; } = null!;
    public User Seller { get; set; } = null!;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
