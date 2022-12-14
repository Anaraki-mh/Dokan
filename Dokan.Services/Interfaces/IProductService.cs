using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IProductService
    {
        Task RestoreAsync(int id);
        Task<Product> CreateAsync(Product entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<Product> entities);
        Task<Product> FindByIdAsync(int id);
        Task<List<Product>> ListAsync();
        Task<List<Product>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task<List<Product>> SearchAsync(string searchString);
        Task UpdateAsync(Product entity);
    }
}