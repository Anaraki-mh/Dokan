using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface ICouponService
    {
        Task AddBackAsync(int id);
        Task<Coupon> CreateAsync(Coupon entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<Coupon> entities);
        Task<Coupon> FindByIdAsync(int id);
        Task<List<Coupon>> ListAsync();
        Task<List<Coupon>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task<Coupon> SearchAsync(string searchString);
        Task UpdateAsync(Coupon entity);
    }
}