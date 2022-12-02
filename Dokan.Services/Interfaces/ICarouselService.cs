using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface ICarouselService
    {
        Task AddBackAsync(int id);
        Task<Carousel> CreateAsync(Carousel entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<Carousel> entities);
        Task<Carousel> FindByIdAsync(int id);
        Task<List<Carousel>> ListAsync();
        Task<List<Carousel>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(Carousel entity);
    }
}