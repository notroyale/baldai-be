using baldai_be.Domain.Enums;

namespace baldai_be.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public decimal Rating { get; set; }
    public decimal WalletBalance { get; set; }
    public DateTime JoinedDate { get; set; }

    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
