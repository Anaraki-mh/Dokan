using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IOrderItemService
    {
        Task<OrderItem> CreateAsync(OrderItem entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<OrderItem> entities);
        Task<OrderItem> FindByIdAsync(int id);
        Task<List<OrderItem>> ListAsync();
        Task UpdateAsync(OrderItem entity);
    }
}