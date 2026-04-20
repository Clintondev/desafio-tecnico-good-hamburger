using FluentAssertions;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Tests.Application;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _store = new();

    public Task<IEnumerable<Order>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Order>>(_store.OrderByDescending(o => o.CreatedAt).ToList());

    public Task<Order?> GetByIdAsync(Guid id) =>
        Task.FromResult(_store.FirstOrDefault(o => o.Id == id));

    public Task AddAsync(Order order)
    {
        _store.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order) => Task.CompletedTask;

    public Task DeleteAsync(Order order)
    {
        _store.Remove(order);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}

public class OrderServiceTests
{
    private static OrderService CreateService() => new(new InMemoryOrderRepository());

    [Fact]
    public async Task CreateAsync_ValidRequest_ShouldReturnOrderWithCorrectValues()
    {
        var service = CreateService();
        var request = new CreateOrderRequest(SandwichType.XBurger, HasFries: true, HasDrink: true);

        var result = await service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Sandwich.Should().Be(SandwichType.XBurger);
        result.Value.DiscountPercent.Should().Be(20);
        result.Value.Total.Should().BeLessThan(result.Value.Subtotal);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentId_ShouldReturnFailure()
    {
        var service = CreateService();

        var result = await service.DeleteAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("nao encontrado");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ShouldReturnFailure()
    {
        var service = CreateService();

        var result = await service.GetByIdAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("nao encontrado");
    }

    [Fact]
    public async Task GetAllAsync_AfterCreatingTwoOrders_ShouldReturnBoth()
    {
        var service = CreateService();
        await service.CreateAsync(new CreateOrderRequest(SandwichType.XBurger, false, false));
        await service.CreateAsync(new CreateOrderRequest(SandwichType.XEgg, true, false));

        var result = await service.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingOrder_ShouldRecalculatePricing()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateOrderRequest(SandwichType.XBurger, false, false));

        var updated = await service.UpdateAsync(
            created.Value!.Id,
            new UpdateOrderRequest(SandwichType.XBacon, true, true));

        updated.IsSuccess.Should().BeTrue();
        updated.Value!.DiscountPercent.Should().Be(20);
        updated.Value.SandwichName.Should().Be("X Bacon");
    }

    [Fact]
    public async Task DeleteAsync_ExistingOrder_ShouldSucceed()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateOrderRequest(SandwichType.XBurger, false, false));

        var deleteResult = await service.DeleteAsync(created.Value!.Id);
        var getResult = await service.GetByIdAsync(created.Value.Id);

        deleteResult.IsSuccess.Should().BeTrue();
        getResult.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void GetMenu_ShouldReturnAllItemsWithCorrectPrices()
    {
        var service = CreateService();

        var menu = service.GetMenu();

        menu.Sandwiches.Should().HaveCount(3);
        menu.Sides.Should().HaveCount(2);
        menu.Sandwiches.Should().Contain(s => s.Name == "X Burger" && s.Price == 5.00m);
        menu.Sides.Should().Contain(s => s.Name == "Batata Frita" && s.Price == 2.00m);
    }
}
