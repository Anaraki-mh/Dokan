using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Core.DataAccess
{
    public interface IRepository<T> where T : class
    {
        T Create(T entity);
        void Delete(T entity);
        void Update(T entity);
        List<T> List();
    }
}
