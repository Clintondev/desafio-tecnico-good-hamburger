using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.DTOs;

public record UpdateOrderRequest(
    SandwichType Sandwich,
    bool HasFries,
    bool HasDrink
);
