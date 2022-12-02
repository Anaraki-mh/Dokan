using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class ProductCommentService : IProductCommentService
    {
        #region Fields and Properties

        private IRepository<ProductComment> _repository { get; }

        #endregion


        #region Constructor

        public ProductCommentService(IRepository<ProductComment> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new ProductComment as a new record in the ProductComment table
        /// </summary>
        /// <param name="entity">The ProductComment that gets added to the table in the database</param>
        /// <returns>A taks of ProductComment</returns>
        public async Task<ProductComment> CreateAsync(ProductComment entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a ProductComment from the ProductComment table
        /// </summary>
        /// <param name="id">The id of the ProductComment that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the ProductComment table
        /// </summary>
        /// <param name="entities">The list of ProductComments that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<ProductComment> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table ProductComment by its id
        /// </summary>
        /// <param name="id">The id of the ProductComment that gets returned</param>
        /// <returns>The (task of) ProductComment that has a matching id with the given id</returns>
        public async Task<ProductComment> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new ProductComment();
            }
        }

        /// <summary>
        /// Returns a list containing all records of ProductComment table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in ProductComment table with IsRemoved of false</returns>
        public async Task<List<ProductComment>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<ProductComment>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of ProductComment table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in ProductComment table with IsRemoved of true</returns>
        public async Task<List<ProductComment>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<ProductComment>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a ProductComment to true
        /// </summary>
        /// <param name="id">The id of the ProductComment which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new ProductComment(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a ProductComment to false
        /// </summary>
        /// <param name="id">The id of the ProductComment which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new ProductComment(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsApproved property of a ProductComment to false
        /// </summary>
        /// <param name="id">The id of the ProductComment which gets its IsApproved property updated</param>
        /// <returns>A task</returns>
        public async Task ApproveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new ProductComment(); ;
            entity.IsApproved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a ProductComment
        /// </summary>
        /// <param name="entity">The modified ProductComment</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(ProductComment entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
