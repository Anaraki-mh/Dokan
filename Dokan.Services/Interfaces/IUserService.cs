using Dokan.Domain.UsersAndRoles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IUserService
    {
        Task<User> CreateAsync(User entity);
        Task DeleteAsync(string id);
        Task DeleteRangeAsync(List<User> entities);
        Task<User> FindByIdAsync(string id);
        Task<List<User>> ListAsync();
        Task<List<User>> SearchAsync(string searchString);
        Task UpdateAsync(User entity);
    }
}