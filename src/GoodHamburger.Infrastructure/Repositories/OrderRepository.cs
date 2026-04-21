using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync() => await _context.Orders
        .OrderByDescending(o => o.CreatedAt)
        .ToListAsync();

    public async Task<Order?> GetByIdAsync(Guid id) => await _context.Orders.FindAsync(id);

    public async Task AddAsync(Order order) => await _context.Orders.AddAsync(order);

    public Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
