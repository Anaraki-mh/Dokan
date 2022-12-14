using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IMessageService
    {
        Task RestoreAsync(int id);
        Task<Message> CreateAsync(Message entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<Message> entities);
        Task<Message> FindByIdAsync(int id);
        Task<List<Message>> ListAsync();
        Task<List<Message>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(Message entity);
    }
}