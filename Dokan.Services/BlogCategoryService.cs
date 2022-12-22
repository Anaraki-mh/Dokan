using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class BlogCategoryService : IBlogCategoryService
    {
        #region Fields and Properties

        private IRepository<BlogCategory> _repository { get; }

        #endregion


        #region Constructor

        public BlogCategoryService(IRepository<BlogCategory> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new BlogCategory as a new record in the BlogCategory table
        /// </summary>
        /// <param name="entity">The BlogCategory that gets added to the table in the database</param>
        /// <returns>A taks of BlogCategory</returns>
        public async Task<BlogCategory> CreateAsync(BlogCategory entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a BlogCategory from the BlogCategory table
        /// </summary>
        /// <param name="id">The id of the BlogCategory that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the BlogCategory table
        /// </summary>
        /// <param name="entities">The list of BlogCategorys that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<BlogCategory> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table BlogCategory by its id
        /// </summary>
        /// <param name="id">The id of the BlogCategory that gets returned</param>
        /// <returns>The (task of) BlogCategory that has a matching id with the given id</returns>
        public async Task<BlogCategory> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new BlogCategory();
            }
        }

        /// <summary>
        /// Returns a list containing all records of BlogCategory table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in BlogCategory table with IsRemoved of false</returns>
        public async Task<List<BlogCategory>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<BlogCategory>();

            return entityList
                .Where(x => !x.IsRemoved)
                .OrderBy(x => x.Priority)
                .ThenByDescending(x => x.UpdateDateTime)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of BlogCategory table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in BlogCategory table with IsRemoved of true</returns>
        public async Task<List<BlogCategory>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<BlogCategory>();

            return entityList
                .Where(x => x.IsRemoved)
                .OrderBy(x => x.Priority)
                .ThenByDescending(x => x.UpdateDateTime)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a BlogCategory to true
        /// </summary>
        /// <param name="id">The id of the BlogCategory which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new BlogCategory(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a BlogCategory to false
        /// </summary>
        /// <param name="id">The id of the BlogCategory which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RestoreAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new BlogCategory(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a BlogCategory
        /// </summary>
        /// <param name="entity">The modified BlogCategory</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(BlogCategory entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
