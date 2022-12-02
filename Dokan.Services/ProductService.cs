using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class ProductService : IProductService
    {
        #region Fields and Properties

        private IRepository<Product> _repository { get; }

        #endregion


        #region Constructor

        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new Product as a new record in the Product table
        /// </summary>
        /// <param name="entity">The Product that gets added to the table in the database</param>
        /// <returns>A taks of Product</returns>
        public async Task<Product> CreateAsync(Product entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a Product from the Product table
        /// </summary>
        /// <param name="id">The id of the Product that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the Product table
        /// </summary>
        /// <param name="entities">The list of Products that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<Product> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table Product by its id
        /// </summary>
        /// <param name="id">The id of the Product that gets returned</param>
        /// <returns>The (task of) Product that has a matching id with the given id</returns>
        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new Product();
            }
        }

        /// <summary>
        /// Returns a list containing all records of Product table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in Product table with IsRemoved of false</returns>
        public async Task<List<Product>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Product>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of Product table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in Product table with IsRemoved of true</returns>
        public async Task<List<Product>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Product>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Product to true
        /// </summary>
        /// <param name="id">The id of the Product which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Product(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Product to false
        /// </summary>
        /// <param name="id">The id of the Product which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Product(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a Product
        /// </summary>
        /// <param name="entity">The modified Product</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(Product entity)
        {
            await _repository.UpdateAsync(entity);
        }


        /// <summary>
        /// Searches for the searchString in the Product's Title and Content
        /// </summary>
        /// <param name="searchString">A string that gets searched for in the Product table</param>
        /// <returns>A list of entities that contain the searchString in their Title or Content</returns>
        public async Task<List<Product>> SearchAsync(string searchString)
        {
            var list = await ListAsync();
            searchString = searchString.ToLower();

            if (!string.IsNullOrEmpty(searchString) && list != null)
            {
                list = list
                    .Where(x => x.Title.ToLower().Contains(searchString) || x.Description.ToLower().Contains(searchString))
                    .ToList();
            }
            else
                list = new List<Product>();

            return list;
        }

        #endregion
    }
}
