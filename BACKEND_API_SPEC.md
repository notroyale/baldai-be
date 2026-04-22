API Specification: Furniture Marketplace Core
Base URL: /api/v1

Auth: Bearer Token (JWT)

1. Authentication & Identity
Endpoint,Method,Role,Description
/auth/register,POST,Public,Registers a new user (Buyer/Seller).
/auth/login,POST,Public,Returns JWT and User metadata.
/users/profile,GET,User,"Gets current user ratings, wallet balance, and role."
2. Product Management (The Listing Flow)
Endpoint,Method,Role,Logic / State Transition
/products,GET,Public,Returns all items with status Active.
/products,POST,Seller,Creates listing. Status set to PendingModeration.
/products/{id},GET,Public,Full item details + dimensions + seller rating.
/products/{id},PUT,Seller,Update details. Re-triggers PendingModeration status.
3. Moderation (Catawiki Curation)
Endpoint,Method,Role,Logic / State Transition
/moderation/pending,GET,Mod,List all items with status PendingModeration.
/moderation/approve/{id},POST,Mod,Sets Product status to Active. Item is now searchable.
/moderation/reject/{id},POST,Mod,Sets Product status to Rejected. Requires Reason in body.
4. Transactions & Escrow (The BPMN Engine)
Endpoint,Method,Role,Logic / State Transition
/orders/checkout,POST,Buyer,State: Authorized. Creates Transaction. Marks Product as Reserved.
/orders/{id},GET,Participant,Returns detailed transaction status and timeline.
/orders/{id}/shipping,POST,Seller,State: Shipped. Requires Tracking Number or Label Scan.
/orders/{id}/delivered,PATCH,System,State: Delivered. Triggered by Webhook. Starts 48h timer.
/orders/{id}/accept,POST,Buyer,State: Completed. Manually triggers payout before timer ends.
5. Logistics & Disputes
Endpoint,Method,Role,Logic / State Transition
/logistics/pickup-code,GET,Buyer,Generates 6-digit QR code for local handover.
/logistics/verify-pickup,POST,Seller,Validates Buyer's code. Sets state to Delivered.
/disputes/open,POST,Buyer,State: Disputed. Pauses 48h timer. Requires photos/desc.
/disputes/{id}/resolve,POST,Mod,Finalizes transaction (Refund or Payout to Seller).
6. Data Contracts (DTOs)
Product Create DTO
JSON
{
  "title": "Mid-Century Modern Sofa",
  "description": "Authentic 1960s velvet sofa...",
  "price": 1200.00,
  "dimensions": {
    "length": 210,
    "width": 90,
    "height": 85
  },
  "images": ["base64_string_or_multipart"],
  "shippingOption": "Integrated | Pickup | Both"
}
Order Status Response DTO
JSON
{
  "orderId": "uuid",
  "status": "Shipped",
  "isEscrowHeld": true,
  "timeline": [
    { "state": "Authorized", "timestamp": "2023-10-01T10:00Z" },
    { "state": "Shipped", "timestamp": "2023-10-02T14:30Z" }
  ],
  "canDispute": false
}