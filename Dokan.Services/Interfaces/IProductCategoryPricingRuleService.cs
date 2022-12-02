using Dokan.Domain.Website;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public interface IProductCategoryPricingRuleService
    {
        Task AddBackAsync(int id);
        Task<ProductCategoryPricingRule> CreateAsync(ProductCategoryPricingRule entity);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(List<ProductCategoryPricingRule> entities);
        Task<ProductCategoryPricingRule> FindByIdAsync(int id);
        Task<List<ProductCategoryPricingRule>> ListAsync();
        Task<List<ProductCategoryPricingRule>> ListOfRemovedAsync();
        Task RemoveAsync(int id);
        Task UpdateAsync(ProductCategoryPricingRule entity);
    }
}