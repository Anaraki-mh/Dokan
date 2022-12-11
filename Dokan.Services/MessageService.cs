using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class MessageService : IMessageService
    {
        #region Fields and Properties

        private IRepository<Message> _repository { get; }

        #endregion


        #region Constructor

        public MessageService(IRepository<Message> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new Message as a new record in the Message table
        /// </summary>
        /// <param name="entity">The Message that gets added to the table in the database</param>
        /// <returns>A taks of Message</returns>
        public async Task<Message> CreateAsync(Message entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a Message from the Message table
        /// </summary>
        /// <param name="id">The id of the Message that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the Message table
        /// </summary>
        /// <param name="entities">The list of Messages that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<Message> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table Message by its id
        /// </summary>
        /// <param name="id">The id of the Message that gets returned</param>
        /// <returns>The (task of) Message that has a matching id with the given id</returns>
        public async Task<Message> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new Message();
            }
        }

        /// <summary>
        /// Returns a list containing all records of Message table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in Message table with IsRemoved of false</returns>
        public async Task<List<Message>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Message>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of Message table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in Message table with IsRemoved of true</returns>
        public async Task<List<Message>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Message>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Message to true
        /// </summary>
        /// <param name="id">The id of the Message which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Message(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Message to false
        /// </summary>
        /// <param name="id">The id of the Message which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Message(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a Message
        /// </summary>
        /// <param name="entity">The modified Message</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(Message entity)
        {
            await _repository.UpdateAsync(entity);
        }

        #endregion
    }
}
