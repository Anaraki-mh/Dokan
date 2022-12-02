using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IKeyValueContentService
    {
        Task AddBackAsync(int id);
        Task<KeyValueContent> CreateAsync(KeyValueContent entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<KeyValueContent> entities);
        Task<KeyValueContent> FindByIdAsync(int id);
        Task<List<KeyValueContent>> ListAsync();
        Task<List<KeyValueContent>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task<List<KeyValueContent>> SearchAsync(string searchString);
        Task UpdateAsync(KeyValueContent entity);
    }
}