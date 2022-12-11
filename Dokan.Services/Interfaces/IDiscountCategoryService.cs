using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IDiscountCategoryService
    {
        Task AddBackAsync(int id);
        Task<DiscountCategory> CreateAsync(DiscountCategory entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<DiscountCategory> entities);
        Task<DiscountCategory> FindByIdAsync(int id);
        Task<List<DiscountCategory>> ListAsync();
        Task<List<DiscountCategory>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(DiscountCategory entity);
    }
}