using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Domain.Entities;

public class Order
{
    public const decimal XBurgerPrice = 5.00m;
    public const decimal XEggPrice = 4.50m;
    public const decimal XBaconPrice = 7.00m;
    public const decimal FriesPrice = 2.00m;
    public const decimal DrinkPrice = 2.50m;

    public Guid Id { get; private set; }
    public SandwichType Sandwich { get; private set; }
    public bool HasFries { get; private set; }
    public bool HasDrink { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Order()
    {
    }

    public static Order Create(SandwichType sandwich, bool hasFries, bool hasDrink)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Sandwich = sandwich,
            HasFries = hasFries,
            HasDrink = hasDrink,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        order.RecalculatePricing();
        return order;
    }

    public static Order Create(SandwichType sandwich, IEnumerable<OrderItemType> sideItems)
    {
        var items = ValidateSideItems(sideItems);
        return Create(
            sandwich,
            items.Contains(OrderItemType.Fries),
            items.Contains(OrderItemType.Drink));
    }

    public void Update(SandwichType sandwich, bool hasFries, bool hasDrink)
    {
        Sandwich = sandwich;
        HasFries = hasFries;
        HasDrink = hasDrink;
        UpdatedAt = DateTime.UtcNow;

        RecalculatePricing();
    }

    public void Update(SandwichType sandwich, IEnumerable<OrderItemType> sideItems)
    {
        var items = ValidateSideItems(sideItems);
        Update(
            sandwich,
            items.Contains(OrderItemType.Fries),
            items.Contains(OrderItemType.Drink));
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    private void RecalculatePricing()
    {
        var sandwichPrice = Sandwich switch
        {
            SandwichType.XBurger => XBurgerPrice,
            SandwichType.XEgg => XEggPrice,
            SandwichType.XBacon => XBaconPrice,
            _ => throw new DomainException($"Sanduiche desconhecido: {Sandwich}")
        };

        Subtotal = sandwichPrice
            + (HasFries ? FriesPrice : 0)
            + (HasDrink ? DrinkPrice : 0);

        Discount = (HasFries, HasDrink) switch
        {
            (true, true) => 20m,
            (false, true) => 15m,
            (true, false) => 10m,
            _ => 0m
        };

        Total = Subtotal * (1 - Discount / 100);
    }

    public static decimal GetSandwichPrice(SandwichType type) => type switch
    {
        SandwichType.XBurger => XBurgerPrice,
        SandwichType.XEgg => XEggPrice,
        SandwichType.XBacon => XBaconPrice,
        _ => throw new DomainException($"Sanduiche desconhecido: {type}")
    };

    private static IReadOnlyCollection<OrderItemType> ValidateSideItems(IEnumerable<OrderItemType> sideItems)
    {
        var items = sideItems.ToList();
        var duplicate = items
            .GroupBy(item => item)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate is not null)
        {
            throw new DuplicateItemException(GetItemName(duplicate.Key));
        }

        return items;
    }

    private static string GetItemName(OrderItemType item) => item switch
    {
        OrderItemType.Fries => "Batata Frita",
        OrderItemType.Drink => "Refrigerante",
        _ => item.ToString()
    };
}
