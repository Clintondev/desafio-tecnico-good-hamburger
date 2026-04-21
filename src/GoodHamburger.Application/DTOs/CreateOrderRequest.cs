using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.DTOs;

public record CreateOrderRequest(
    SandwichType Sandwich,
    bool HasFries,
    bool HasDrink,
    IReadOnlyCollection<OrderItemType>? Items = null
);
