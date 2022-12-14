using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IMenuService
    {
        Task RestoreAsync(int id);
        Task<Menu> CreateAsync(Menu entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<Menu> entities);
        Task<Menu> FindByIdAsync(int id);
        Task<List<Menu>> ListAsync();
        Task<List<Menu>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(Menu entity);
    }
}