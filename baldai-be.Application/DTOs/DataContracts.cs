namespace baldai_be.Application.DTOs;

// --- Auth & Users ---
public record RegisterDto(string Email, string Password, string FirstName, string LastName, string Role);
public record LoginDto(string Email, string Password);
public record UserProfileDto(Guid Id, string Email, string Role, decimal Rating, decimal WalletBalance, DateTime JoinedDate);

// --- Products ---
public record DimensionsDto(decimal Length, decimal Width, decimal Height);

public record ProductCreateDto(
    string Title, 
    string Description, 
    decimal Price, 
    DimensionsDto Dimensions, 
    List<string> Images, 
    string ShippingOption
);

public record ProductSummaryDto(Guid Id, string Title, decimal Price, string PrimaryImage, string Status);
public record PagedResult<T>(List<T> Items, int TotalCount);

public record SellerDto(Guid Id, string Name, decimal Rating);
public record ProductDetailDto(
    Guid Id, 
    string Title, 
    string Description, 
    decimal Price, 
    DimensionsDto Dimensions, 
    List<string> Images, 
    string ShippingOption, 
    SellerDto Seller, 
    string Status
);

// --- Orders / Escrow ---
public record CheckoutDto(Guid ProductId, string LogisticsMethod, Guid? OfferId = null);
public record TransactionSummaryDto(Guid TransactionId, string Status);

public record OrderStatusTimelineDto(string State, string Timestamp);

public record OrderStateDto(
    Guid OrderId, 
    string Status, 
    bool IsEscrowHeld, 
    string DeliveryMethod,
    List<OrderStatusTimelineDto> Timeline
);

// --- Logistics & Disputes ---
public record ShippingUpdateDto(string TrackingNumber, string Carrier);
public record VerifyPickupDto(string Code);
public record OpenDisputeDto(Guid OrderId, string Reason, List<string> ImageEvidenceUrls);
public record ResolveDisputeDto(string Resolution, string Notes);
public record RejectDto(string Reason);

// --- Chat & Negotiation ---
public record ChatThreadDto(Guid Id, Guid ProductId, string ProductTitle, Guid BuyerId, string BuyerName, Guid SellerId, string SellerName, MessageDto? LastMessage);
public record MessageDto(Guid Id, Guid SenderId, string Content, DateTime SentAt, OfferDto? Offer);
public record OfferDto(Guid Id, decimal Amount, string Status, DateTime CreatedAt, DateTime ExpiresAt);

public record CreateMessageDto(Guid ProductId, string Content);
public record CreateOfferDto(Guid ProductId, decimal Amount, string MessageContent);
