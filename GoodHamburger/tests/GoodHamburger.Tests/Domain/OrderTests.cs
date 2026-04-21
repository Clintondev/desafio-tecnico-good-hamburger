using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Tests.Domain;

public class OrderTests
{
    [Fact]
    public void Create_WithSandwichOnly_ShouldHaveZeroDiscount()
    {
        var order = Order.Create(SandwichType.XBurger, hasFries: false, hasDrink: false);

        order.Discount.Should().Be(0);
        order.Subtotal.Should().Be(Order.XBurgerPrice);
        order.Total.Should().Be(Order.XBurgerPrice);
    }

    [Fact]
    public void Create_WithFriesAndDrink_ShouldApply20PercentDiscount()
    {
        var order = Order.Create(SandwichType.XBurger, hasFries: true, hasDrink: true);

        var expectedSubtotal = Order.XBurgerPrice + Order.FriesPrice + Order.DrinkPrice;
        var expectedTotal = expectedSubtotal * 0.80m;

        order.Discount.Should().Be(20);
        order.Subtotal.Should().Be(expectedSubtotal);
        order.Total.Should().BeApproximately(expectedTotal, 0.01m);
    }

    [Fact]
    public void Create_WithDrinkOnly_ShouldApply15PercentDiscount()
    {
        var order = Order.Create(SandwichType.XEgg, hasFries: false, hasDrink: true);

        var expectedSubtotal = Order.XEggPrice + Order.DrinkPrice;

        order.Discount.Should().Be(15);
        order.Subtotal.Should().Be(expectedSubtotal);
        order.Total.Should().BeApproximately(expectedSubtotal * 0.85m, 0.01m);
    }

    [Fact]
    public void Create_WithFriesOnly_ShouldApply10PercentDiscount()
    {
        var order = Order.Create(SandwichType.XBacon, hasFries: true, hasDrink: false);

        var expectedSubtotal = Order.XBaconPrice + Order.FriesPrice;

        order.Discount.Should().Be(10);
        order.Subtotal.Should().Be(expectedSubtotal);
        order.Total.Should().BeApproximately(expectedSubtotal * 0.90m, 0.01m);
    }

    [Theory]
    [InlineData(SandwichType.XBurger, 5.00)]
    [InlineData(SandwichType.XEgg, 4.50)]
    [InlineData(SandwichType.XBacon, 7.00)]
    public void Create_ShouldUseCorrectSandwichPrice(SandwichType type, decimal expectedPrice)
    {
        var order = Order.Create(type, hasFries: false, hasDrink: false);

        order.Subtotal.Should().Be(expectedPrice);
        order.Total.Should().Be(expectedPrice);
    }

    [Fact]
    public void Update_ChangingItems_ShouldRecalculatePricing()
    {
        var order = Order.Create(SandwichType.XBurger, hasFries: false, hasDrink: false);

        order.Update(SandwichType.XBacon, hasFries: true, hasDrink: true);

        var expectedSubtotal = Order.XBaconPrice + Order.FriesPrice + Order.DrinkPrice;

        order.Discount.Should().Be(20);
        order.Subtotal.Should().Be(expectedSubtotal);
        order.Total.Should().BeApproximately(expectedSubtotal * 0.80m, 0.01m);
        order.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldSetStatusToPending()
    {
        var order = Order.Create(SandwichType.XBurger, hasFries: false, hasDrink: false);

        order.Status.Should().Be(OrderStatus.Pending);
        order.Id.Should().NotBe(Guid.Empty);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithAllItems_SubtotalShouldSumAllPrices()
    {
        var order = Order.Create(SandwichType.XBacon, hasFries: true, hasDrink: true);

        var expectedSubtotal = Order.XBaconPrice + Order.FriesPrice + Order.DrinkPrice;
        order.Subtotal.Should().Be(expectedSubtotal);
    }

    [Fact]
    public void Create_WithDuplicatedSideItems_ShouldThrowDuplicateItemException()
    {
        var act = () => Order.Create(
            SandwichType.XBurger,
            new[] { OrderItemType.Fries, OrderItemType.Fries });

        act.Should()
            .Throw<DuplicateItemException>()
            .WithMessage("*Batata Frita*");
    }
}
