using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IBlogPostService
    {
        Task AddBackAsync(int id);
        Task<BlogPost> CreateAsync(BlogPost entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<BlogPost> entities);
        Task<BlogPost> FindByIdAsync(int id);
        Task<List<BlogPost>> ListAsync();
        Task<List<BlogPost>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task<List<BlogPost>> SearchAsync(string searchString);
        Task UpdateAsync(BlogPost entity);
    }
}