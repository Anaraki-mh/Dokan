using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IProductPricingRuleService
    {
        Task AddBackAsync(int id);
        Task<ProductPricingRule> CreateAsync(ProductPricingRule entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<ProductPricingRule> entities);
        Task<ProductPricingRule> FindByIdAsync(int id);
        Task<List<ProductPricingRule>> ListAsync();
        Task<List<ProductPricingRule>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(ProductPricingRule entity);
    }
}