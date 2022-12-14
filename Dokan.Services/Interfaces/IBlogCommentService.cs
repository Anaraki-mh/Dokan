using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IBlogCommentService
    {
        Task RestoreAsync(int id);
        Task ApproveAsync(int id);
        Task<BlogComment> CreateAsync(BlogComment entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<BlogComment> entities);
        Task<BlogComment> FindByIdAsync(int id);
        Task<List<BlogComment>> ListAsync();
        Task<List<BlogComment>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(BlogComment entity);
    }
}