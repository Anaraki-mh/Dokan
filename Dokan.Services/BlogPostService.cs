using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class BlogPostService : IBlogPostService
    {
        #region Fields and Properties

        private IRepository<BlogPost> _repository { get; }

        #endregion


        #region Constructor

        public BlogPostService(IRepository<BlogPost> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new BlogPost as a new record in the BlogPost table
        /// </summary>
        /// <param name="entity">The BlogPost that gets added to the table in the database</param>
        /// <returns>A taks of BlogPost</returns>
        public async Task<BlogPost> CreateAsync(BlogPost entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a BlogPost from the BlogPost table
        /// </summary>
        /// <param name="id">The id of the BlogPost that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the BlogPost table
        /// </summary>
        /// <param name="entities">The list of BlogPosts that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<BlogPost> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table BlogPost by its id
        /// </summary>
        /// <param name="id">The id of the BlogPost that gets returned</param>
        /// <returns>The (task of) BlogPost that has a matching id with the given id</returns>
        public async Task<BlogPost> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new BlogPost();
            }
        }

        /// <summary>
        /// Returns a list containing all records of BlogPost table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in BlogPost table with IsRemoved of false</returns>
        public async Task<List<BlogPost>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<BlogPost>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of BlogPost table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in BlogPost table with IsRemoved of true</returns>
        public async Task<List<BlogPost>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<BlogPost>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a BlogPost to true
        /// </summary>
        /// <param name="id">The id of the BlogPost which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new BlogPost(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a BlogPost to false
        /// </summary>
        /// <param name="id">The id of the BlogPost which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new BlogPost(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a BlogPost
        /// </summary>
        /// <param name="entity">The modified BlogPost</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(BlogPost entity)
        {
            await _repository.UpdateAsync(entity);
        }


        /// <summary>
        /// Searches for the searchString in the BlogPost's Title and Content
        /// </summary>
        /// <param name="searchString">A string that gets searched for in the BlogPost table</param>
        /// <returns>A list of entities that contain the searchString in their Title or Content</returns>
        public async Task<List<BlogPost>> SearchAsync(string searchString)
        {
            var list = await ListAsync();
            searchString = searchString.ToLower();

            if (!string.IsNullOrEmpty(searchString) && list != null)
            {
                list = list
                    .Where(x => x.Title.ToLower().Contains(searchString) || x.Content.ToLower().Contains(searchString))
                    .ToList();
            }
            else
                list = new List<BlogPost>();

            return list;
        }

        #endregion
    }
}
