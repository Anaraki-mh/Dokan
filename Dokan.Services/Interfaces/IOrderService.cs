using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IOrderService
    {
        Task<Order> CreateAsync(Order entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<Order> entities);
        Task<Order> FindByIdAsync(int id);
        Task<List<Order>> ListAsync();
        Task UpdateAsync(Order entity);
    }
}