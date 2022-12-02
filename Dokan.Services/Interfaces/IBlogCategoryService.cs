using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IBlogCategoryService
    {
        Task AddBackAsync(int id);
        Task<BlogCategory> CreateAsync(BlogCategory entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<BlogCategory> entities);
        Task<BlogCategory> FindByIdAsync(int id);
        Task<List<BlogCategory>> ListAsync();
        Task<List<BlogCategory>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(BlogCategory entity);
    }
}