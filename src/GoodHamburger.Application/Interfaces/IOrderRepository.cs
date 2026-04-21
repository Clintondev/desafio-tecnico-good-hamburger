using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(Guid id);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
    Task SaveChangesAsync();
}
