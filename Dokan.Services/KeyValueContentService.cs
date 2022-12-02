using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class KeyValueContentService : IKeyValueContentService
    {
        #region Fields and Properties

        private IRepository<KeyValueContent> _repository { get; }

        #endregion


        #region Constructor

        public KeyValueContentService(IRepository<KeyValueContent> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new KeyValueContent as a new record in the KeyValueContent table
        /// </summary>
        /// <param name="entity">The KeyValueContent that gets added to the table in the database</param>
        /// <returns>A taks of KeyValueContent</returns>
        public async Task<KeyValueContent> CreateAsync(KeyValueContent entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a KeyValueContent from the KeyValueContent table
        /// </summary>
        /// <param name="id">The id of the KeyValueContent that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the KeyValueContent table
        /// </summary>
        /// <param name="entities">The list of KeyValueContents that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<KeyValueContent> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table KeyValueContent by its id
        /// </summary>
        /// <param name="id">The id of the KeyValueContent that gets returned</param>
        /// <returns>The (task of) KeyValueContent that has a matching id with the given id</returns>
        public async Task<KeyValueContent> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new KeyValueContent();
            }
        }

        /// <summary>
        /// Returns a list containing all records of KeyValueContent table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in KeyValueContent table with IsRemoved of false</returns>
        public async Task<List<KeyValueContent>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<KeyValueContent>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of KeyValueContent table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in KeyValueContent table with IsRemoved of true</returns>
        public async Task<List<KeyValueContent>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<KeyValueContent>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a KeyValueContent to true
        /// </summary>
        /// <param name="id">The id of the KeyValueContent which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new KeyValueContent(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a KeyValueContent to false
        /// </summary>
        /// <param name="id">The id of the KeyValueContent which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new KeyValueContent(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a KeyValueContent
        /// </summary>
        /// <param name="entity">The modified KeyValueContent</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(KeyValueContent entity)
        {
            await _repository.UpdateAsync(entity);
        }


        /// <summary>
        /// Searches for the searchString in the KeyValueContent's Title and Content
        /// </summary>
        /// <param name="searchString">A string that gets searched for in the KeyValueContent table</param>
        /// <returns>A list of entities that contain the searchString in their Title or Content</returns>
        public async Task<List<KeyValueContent>> SearchAsync(string searchString)
        {
            var list = await ListAsync();
            searchString = searchString.ToLower();

            if (!string.IsNullOrEmpty(searchString) && list != null)
            {
                list = list
                    .Where(x => x.Description.ToLower().Contains(searchString))
                    .ToList();
            }
            else
                list = new List<KeyValueContent>();

            return list;
        }

        #endregion
    }
}
