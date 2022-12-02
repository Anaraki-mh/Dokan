using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class BlogCommentService : IBlogCommentService
    {
        #region Fields and Properties

        private IRepository<BlogComment> _repository { get; }

        #endregion


        #region Constructor

        public BlogCommentService(IRepository<BlogComment> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new BlogComment as a new record in the BlogComment table
        /// </summary>
        /// <param name="entity">The BlogComment that gets added to the table in the database</param>
        /// <returns>A taks of BlogComment</returns>
        public async Task<BlogComment> CreateAsync(BlogComment entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a BlogComment from the BlogComment table
        /// </summary>
        /// <param name="id">The id of the BlogComment that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the BlogComment table
        /// </summary>
        /// <param name="entities">The list of BlogComments that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<BlogComment> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table BlogComment by its id
        /// </summary>
        /// <param name="id">The id of the BlogComment that gets returned</param>
        /// <returns>The (task of) BlogComment that has a matching id with the given id</returns>
        public async Task<BlogComment> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new BlogComment();
            }
        }

        /// <summary>
        /// Returns a list containing all records of BlogComment table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in BlogComment table with IsRemoved of false</returns>
        public async Task<List<BlogComment>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<BlogComment>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of BlogComment table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in BlogComment table with IsRemoved of true</returns>
        public async Task<List<BlogComment>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<BlogComment>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a BlogComment to true
        /// </summary>
        /// <param name="id">The id of the BlogComment which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new BlogComment(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a BlogComment to false
        /// </summary>
        /// <param name="id">The id of the BlogComment which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new BlogComment(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsApproved property of a BlogComment to false
        /// </summary>
        /// <param name="id">The id of the BlogComment which gets its IsApproved property updated</param>
        /// <returns>A task</returns>
        public async Task ApproveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new BlogComment(); ;
            entity.IsApproved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a BlogComment
        /// </summary>
        /// <param name="entity">The modified BlogComment</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(BlogComment entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
