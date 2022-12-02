using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class ProductPricingRuleService : IProductPricingRuleService
    {
        #region Fields and Properties

        private IRepository<ProductPricingRule> _repository { get; }

        #endregion


        #region Constructor

        public ProductPricingRuleService(IRepository<ProductPricingRule> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new ProductPricingRule as a new record in the ProductPricingRule table
        /// </summary>
        /// <param name="entity">The ProductPricingRule that gets added to the table in the database</param>
        /// <returns>A taks of ProductPricingRule</returns>
        public async Task<ProductPricingRule> CreateAsync(ProductPricingRule entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a ProductPricingRule from the ProductPricingRule table
        /// </summary>
        /// <param name="id">The id of the ProductPricingRule that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the ProductPricingRule table
        /// </summary>
        /// <param name="entities">The list of ProductPricingRules that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<ProductPricingRule> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table ProductPricingRule by its id
        /// </summary>
        /// <param name="id">The id of the ProductPricingRule that gets returned</param>
        /// <returns>The (task of) ProductPricingRule that has a matching id with the given id</returns>
        public async Task<ProductPricingRule> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new ProductPricingRule();
            }
        }

        /// <summary>
        /// Returns a list containing all records of ProductPricingRule table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in ProductPricingRule table with IsRemoved of false</returns>
        public async Task<List<ProductPricingRule>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<ProductPricingRule>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of ProductPricingRule table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in ProductPricingRule table with IsRemoved of true</returns>
        public async Task<List<ProductPricingRule>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<ProductPricingRule>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a ProductPricingRule to true
        /// </summary>
        /// <param name="id">The id of the ProductPricingRule which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new ProductPricingRule(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a ProductPricingRule to false
        /// </summary>
        /// <param name="id">The id of the ProductPricingRule which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new ProductPricingRule(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a ProductPricingRule
        /// </summary>
        /// <param name="entity">The modified ProductPricingRule</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(ProductPricingRule entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
