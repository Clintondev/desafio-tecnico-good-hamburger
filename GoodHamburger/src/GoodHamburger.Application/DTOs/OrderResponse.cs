using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.DTOs;

public record OrderResponse(
    Guid Id,
    SandwichType Sandwich,
    string SandwichName,
    bool HasFries,
    bool HasDrink,
    decimal Subtotal,
    decimal DiscountPercent,
    decimal DiscountAmount,
    decimal Total,
    OrderStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
