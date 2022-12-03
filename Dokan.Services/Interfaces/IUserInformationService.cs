using Dokan.Domain.UsersAndRoles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IUserInformationService
    {
        Task<UserInformation> CreateAsync(UserInformation entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<UserInformation> entities);
        Task<UserInformation> FindByIdAsync(int id);
        Task<List<UserInformation>> ListAsync();
        Task UpdateAsync(UserInformation entity);
    }
}