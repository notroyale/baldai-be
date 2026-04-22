namespace baldai_be.Domain.Enums;

public enum UserRole
{
    Buyer,
    Seller,
    Admin,
    Moderator
}

public enum ProductStatus
{
    Draft,
    PendingModeration,
    Active,
    Reserved,
    Sold,
    Rejected
}

public enum TransactionStatus
{
    Authorized,
    Shipped,
    Delivered,
    Disputed,
    Completed
}

public enum ShippingOption
{
    Integrated,
    Pickup,
    Both
}

public enum OfferStatus
{
    Pending,
    Accepted,
    Rejected,
    Expired
}
