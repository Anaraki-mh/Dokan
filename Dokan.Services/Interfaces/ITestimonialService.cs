using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface ITestimonialService
    {
        Task RestoreAsync(int id);
        Task<Testimonial> CreateAsync(Testimonial entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<Testimonial> entities);
        Task<Testimonial> FindByIdAsync(int id);
        Task<List<Testimonial>> ListAsync();
        Task<List<Testimonial>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(Testimonial entity);
    }
}