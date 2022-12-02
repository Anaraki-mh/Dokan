using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IProductCommentService
    {
        Task AddBackAsync(int id);
        Task ApproveAsync(int id);
        Task<ProductComment> CreateAsync(ProductComment entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<ProductComment> entities);
        Task<ProductComment> FindByIdAsync(int id);
        Task<List<ProductComment>> ListAsync();
        Task<List<ProductComment>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(ProductComment entity);
    }
}