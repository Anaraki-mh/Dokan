using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface ILogService
    {
        Task<Log> CreateAsync(Log entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<Log> entities);
        Task<Log> FindByIdAsync(int id);
        Task<List<Log>> ListAsync();
        Task UpdateAsync(Log entity);
    }
}