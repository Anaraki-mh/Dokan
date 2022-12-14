using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class TaxCategoryService : ITaxCategoryService
    {
        #region Fields and Properties

        private IRepository<TaxCategory> _repository { get; }

        #endregion


        #region Constructor

        public TaxCategoryService(IRepository<TaxCategory> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new TaxCategory as a new record in the TaxCategory table
        /// </summary>
        /// <param name="entity">The TaxCategory that gets added to the table in the database</param>
        /// <returns>A taks of TaxCategory</returns>
        public async Task<TaxCategory> CreateAsync(TaxCategory entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a TaxCategory from the TaxCategory table
        /// </summary>
        /// <param name="id">The id of the TaxCategory that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the TaxCategory table
        /// </summary>
        /// <param name="entities">The list of ProductCategoryPricingRules that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<TaxCategory> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table TaxCategory by its id
        /// </summary>
        /// <param name="id">The id of the TaxCategory that gets returned</param>
        /// <returns>The (task of) TaxCategory that has a matching id with the given id</returns>
        public async Task<TaxCategory> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new TaxCategory();
            }
        }

        /// <summary>
        /// Returns a list containing all records of TaxCategory table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in TaxCategory table with IsRemoved of false</returns>
        public async Task<List<TaxCategory>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<TaxCategory>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of TaxCategory table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in TaxCategory table with IsRemoved of true</returns>
        public async Task<List<TaxCategory>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<TaxCategory>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a TaxCategory to true
        /// </summary>
        /// <param name="id">The id of the TaxCategory which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new TaxCategory(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a TaxCategory to false
        /// </summary>
        /// <param name="id">The id of the TaxCategory which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RestoreAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new TaxCategory(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a TaxCategory
        /// </summary>
        /// <param name="entity">The modified TaxCategory</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(TaxCategory entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
