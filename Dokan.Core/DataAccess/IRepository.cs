using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Core.DataAccess
{
    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task DeleteAsync(T entity);
        Task UpdateAsync(T entity);
        Task<List<T>> ListAsync();
    }
}
