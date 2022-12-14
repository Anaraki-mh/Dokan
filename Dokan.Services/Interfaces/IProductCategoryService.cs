using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IProductCategoryService
    {
        Task RestoreAsync(int id);
        Task<ProductCategory> CreateAsync(ProductCategory entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<ProductCategory> entities);
        Task<ProductCategory> FindByIdAsync(int id);
        Task<List<ProductCategory>> ListAsync();
        Task<List<ProductCategory>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(ProductCategory entity);
    }
}