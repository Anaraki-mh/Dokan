using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class DiscountCategoryService : IDiscountCategoryService
    {
        #region Fields and Properties

        private IRepository<DiscountCategory> _repository { get; }

        #endregion


        #region Constructor

        public DiscountCategoryService(IRepository<DiscountCategory> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new DiscountCategory as a new record in the DiscountCategory table
        /// </summary>
        /// <param name="entity">The DiscountCategory that gets added to the table in the database</param>
        /// <returns>A taks of DiscountCategory</returns>
        public async Task<DiscountCategory> CreateAsync(DiscountCategory entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a DiscountCategory from the DiscountCategory table
        /// </summary>
        /// <param name="id">The id of the DiscountCategory that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the DiscountCategory table
        /// </summary>
        /// <param name="entities">The list of ProductPricingRules that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<DiscountCategory> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table DiscountCategory by its id
        /// </summary>
        /// <param name="id">The id of the DiscountCategory that gets returned</param>
        /// <returns>The (task of) DiscountCategory that has a matching id with the given id</returns>
        public async Task<DiscountCategory> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new DiscountCategory();
            }
        }

        /// <summary>
        /// Returns a list containing all records of DiscountCategory table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in DiscountCategory table with IsRemoved of false</returns>
        public async Task<List<DiscountCategory>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<DiscountCategory>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of DiscountCategory table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in DiscountCategory table with IsRemoved of true</returns>
        public async Task<List<DiscountCategory>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<DiscountCategory>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a DiscountCategory to true
        /// </summary>
        /// <param name="id">The id of the DiscountCategory which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new DiscountCategory(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a DiscountCategory to false
        /// </summary>
        /// <param name="id">The id of the DiscountCategory which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RestoreAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new DiscountCategory(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a DiscountCategory
        /// </summary>
        /// <param name="entity">The modified DiscountCategory</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(DiscountCategory entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
