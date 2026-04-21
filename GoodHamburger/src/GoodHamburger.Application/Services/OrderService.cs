using GoodHamburger.Application.Common;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<OrderResponse>>> GetAllAsync()
    {
        var orders = await _repository.GetAllAsync();
        return Result<IEnumerable<OrderResponse>>.Success(orders.Select(MapToResponse));
    }

    public async Task<Result<OrderResponse>> GetByIdAsync(Guid id)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order is null)
        {
            return Result<OrderResponse>.Failure($"Pedido '{id}' nao encontrado.", ErrorCode.NotFound);
        }

        return Result<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<Result<OrderResponse>> CreateAsync(CreateOrderRequest request)
    {
        try
        {
            var order = request.Items is null
                ? Order.Create(request.Sandwich, request.HasFries, request.HasDrink)
                : Order.Create(request.Sandwich, request.Items);

            await _repository.AddAsync(order);
            await _repository.SaveChangesAsync();
            return Result<OrderResponse>.Success(MapToResponse(order));
        }
        catch (DomainException ex)
        {
            return Result<OrderResponse>.Failure(ex.Message);
        }
    }

    public async Task<Result<OrderResponse>> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order is null)
        {
            return Result<OrderResponse>.Failure($"Pedido '{id}' nao encontrado.", ErrorCode.NotFound);
        }

        try
        {
            if (request.Items is null)
            {
                order.Update(request.Sandwich, request.HasFries, request.HasDrink);
            }
            else
            {
                order.Update(request.Sandwich, request.Items);
            }

            await _repository.UpdateAsync(order);
            await _repository.SaveChangesAsync();
            return Result<OrderResponse>.Success(MapToResponse(order));
        }
        catch (DomainException ex)
        {
            return Result<OrderResponse>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order is null)
        {
            return Result.Failure($"Pedido '{id}' nao encontrado.", ErrorCode.NotFound);
        }

        await _repository.DeleteAsync(order);
        await _repository.SaveChangesAsync();
        return Result.Success();
    }

    public MenuResponse GetMenu()
    {
        var sandwiches = new List<MenuItemResponse>
        {
            new("x-burger", "X Burger", "Sanduiche", Order.XBurgerPrice),
            new("x-egg", "X Egg", "Sanduiche", Order.XEggPrice),
            new("x-bacon", "X Bacon", "Sanduiche", Order.XBaconPrice)
        };

        var sides = new List<MenuItemResponse>
        {
            new("fries", "Batata Frita", "Acompanhamento", Order.FriesPrice),
            new("drink", "Refrigerante", "Bebida", Order.DrinkPrice)
        };

        return new MenuResponse(sandwiches, sides);
    }

    private static OrderResponse MapToResponse(Order order)
    {
        var sandwichName = order.Sandwich switch
        {
            SandwichType.XBurger => "X Burger",
            SandwichType.XEgg => "X Egg",
            SandwichType.XBacon => "X Bacon",
            _ => order.Sandwich.ToString()
        };

        var discountAmount = Math.Round(order.Subtotal * order.Discount / 100, 2);

        return new OrderResponse(
            order.Id,
            order.Sandwich,
            sandwichName,
            order.HasFries,
            order.HasDrink,
            Math.Round(order.Subtotal, 2),
            order.Discount,
            discountAmount,
            Math.Round(order.Total, 2),
            order.Status,
            order.CreatedAt,
            order.UpdatedAt
        );
    }
}
