using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        #region Fields and Properties

        private IRepository<ProductCategory> _repository { get; }

        #endregion


        #region Constructor

        public ProductCategoryService(IRepository<ProductCategory> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new ProductCategory as a new record in the ProductCategory table
        /// </summary>
        /// <param name="entity">The ProductCategory that gets added to the table in the database</param>
        /// <returns>A taks of ProductCategory</returns>
        public async Task<ProductCategory> CreateAsync(ProductCategory entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a ProductCategory from the ProductCategory table
        /// </summary>
        /// <param name="id">The id of the ProductCategory that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the ProductCategory table
        /// </summary>
        /// <param name="entities">The list of ProductCategorys that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<ProductCategory> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table ProductCategory by its id
        /// </summary>
        /// <param name="id">The id of the ProductCategory that gets returned</param>
        /// <returns>The (task of) ProductCategory that has a matching id with the given id</returns>
        public async Task<ProductCategory> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new ProductCategory();
            }
        }

        /// <summary>
        /// Returns a list containing all records of ProductCategory table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in ProductCategory table with IsRemoved of false</returns>
        public async Task<List<ProductCategory>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<ProductCategory>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of ProductCategory table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in ProductCategory table with IsRemoved of true</returns>
        public async Task<List<ProductCategory>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<ProductCategory>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a ProductCategory to true
        /// </summary>
        /// <param name="id">The id of the ProductCategory which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new ProductCategory(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a ProductCategory to false
        /// </summary>
        /// <param name="id">The id of the ProductCategory which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new ProductCategory(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a ProductCategory
        /// </summary>
        /// <param name="entity">The modified ProductCategory</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(ProductCategory entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
