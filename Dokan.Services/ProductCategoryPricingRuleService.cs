using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class ProductCategoryPricingRuleService : IProductCategoryPricingRuleService
    {
        #region Fields and Properties

        private IRepository<ProductCategoryPricingRule> _repository { get; }

        #endregion


        #region Constructor

        public ProductCategoryPricingRuleService(IRepository<ProductCategoryPricingRule> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new ProductCategoryPricingRule as a new record in the ProductCategoryPricingRule table
        /// </summary>
        /// <param name="entity">The ProductCategoryPricingRule that gets added to the table in the database</param>
        /// <returns>A taks of ProductCategoryPricingRule</returns>
        public async Task<ProductCategoryPricingRule> CreateAsync(ProductCategoryPricingRule entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a ProductCategoryPricingRule from the ProductCategoryPricingRule table
        /// </summary>
        /// <param name="id">The id of the ProductCategoryPricingRule that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the ProductCategoryPricingRule table
        /// </summary>
        /// <param name="entities">The list of ProductCategoryPricingRules that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<ProductCategoryPricingRule> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table ProductCategoryPricingRule by its id
        /// </summary>
        /// <param name="id">The id of the ProductCategoryPricingRule that gets returned</param>
        /// <returns>The (task of) ProductCategoryPricingRule that has a matching id with the given id</returns>
        public async Task<ProductCategoryPricingRule> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new ProductCategoryPricingRule();
            }
        }

        /// <summary>
        /// Returns a list containing all records of ProductCategoryPricingRule table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in ProductCategoryPricingRule table with IsRemoved of false</returns>
        public async Task<List<ProductCategoryPricingRule>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<ProductCategoryPricingRule>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of ProductCategoryPricingRule table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in ProductCategoryPricingRule table with IsRemoved of true</returns>
        public async Task<List<ProductCategoryPricingRule>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<ProductCategoryPricingRule>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a ProductCategoryPricingRule to true
        /// </summary>
        /// <param name="id">The id of the ProductCategoryPricingRule which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new ProductCategoryPricingRule(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a ProductCategoryPricingRule to false
        /// </summary>
        /// <param name="id">The id of the ProductCategoryPricingRule which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new ProductCategoryPricingRule(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a ProductCategoryPricingRule
        /// </summary>
        /// <param name="entity">The modified ProductCategoryPricingRule</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(ProductCategoryPricingRule entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
