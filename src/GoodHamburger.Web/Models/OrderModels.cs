using System.Text.Json.Serialization;

namespace GoodHamburger.Web.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SandwichType
{
    XBurger = 1,
    XEgg = 2,
    XBacon = 3
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Delivered = 3,
    Cancelled = 4
}

public record MenuItemModel(string Id, string Name, string Category, decimal Price);

public record MenuModel(List<MenuItemModel> Sandwiches, List<MenuItemModel> Sides);

public record OrderModel(
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

public record CreateOrderModel(SandwichType Sandwich, bool HasFries, bool HasDrink);
