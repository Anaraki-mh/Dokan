using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IFileService
    {
        Task<File> CreateAsync(File entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<File> entities);
        Task<File> FindByIdAsync(int id);
        Task<List<File>> ListAsync();
        Task<List<File>> SearchAsync(string searchString);
        Task UpdateAsync(File entity);
    }
}