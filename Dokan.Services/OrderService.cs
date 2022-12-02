using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class OrderService : IOrderService
    {
        #region Fields and Properties

        private IRepository<Order> _repository { get; }

        #endregion


        #region Constructor

        public OrderService(IRepository<Order> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new Order as a new record in the Order table
        /// </summary>
        /// <param name="entity">The Order that gets added to the table in the database</param>
        /// <returns>A taks of Order</returns>
        public async Task<Order> CreateAsync(Order entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a Order from the Order table
        /// </summary>
        /// <param name="id">The id of the Order that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the Order table
        /// </summary>
        /// <param name="entities">The list of Orders that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<Order> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table Order by its id
        /// </summary>
        /// <param name="id">The id of the Order that gets returned</param>
        /// <returns>The (task of) Order that has a matching id with the given id</returns>
        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new Order();
            }
        }

        /// <summary>
        /// Returns a list containing all records of Order table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in Order table with IsRemoved of false</returns>
        public async Task<List<Order>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Order>();

            return entityList;
        }

        /// <summary>
        /// Updates a Order
        /// </summary>
        /// <param name="entity">The modified Order</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(Order entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
