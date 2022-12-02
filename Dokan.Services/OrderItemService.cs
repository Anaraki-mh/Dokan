using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class OrderItemService : IOrderItemService
    {
        #region Fields and Properties

        private IRepository<OrderItem> _repository { get; }

        #endregion


        #region Constructor

        public OrderItemService(IRepository<OrderItem> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new OrderItem as a new record in the OrderItem table
        /// </summary>
        /// <param name="entity">The OrderItem that gets added to the table in the database</param>
        /// <returns>A taks of OrderItem</returns>
        public async Task<OrderItem> CreateAsync(OrderItem entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a OrderItem from the OrderItem table
        /// </summary>
        /// <param name="id">The id of the OrderItem that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the OrderItem table
        /// </summary>
        /// <param name="entities">The list of OrderItems that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<OrderItem> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table OrderItem by its id
        /// </summary>
        /// <param name="id">The id of the OrderItem that gets returned</param>
        /// <returns>The (task of) OrderItem that has a matching id with the given id</returns>
        public async Task<OrderItem> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new OrderItem();
            }
        }

        /// <summary>
        /// Returns a list containing all records of OrderItem table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in OrderItem table with IsRemoved of false</returns>
        public async Task<List<OrderItem>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<OrderItem>();

            return entityList;
        }

        /// <summary>
        /// Updates a OrderItem
        /// </summary>
        /// <param name="entity">The modified OrderItem</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(OrderItem entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
