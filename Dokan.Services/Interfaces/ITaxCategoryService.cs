using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface ITaxCategoryService
    {
        Task RestoreAsync(int id);
        Task<TaxCategory> CreateAsync(TaxCategory entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<TaxCategory> entities);
        Task<TaxCategory> FindByIdAsync(int id);
        Task<List<TaxCategory>> ListAsync();
        Task<List<TaxCategory>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(TaxCategory entity);
    }
}