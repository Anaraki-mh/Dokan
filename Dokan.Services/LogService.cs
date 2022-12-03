using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class LogService : ILogService
    {
        #region Fields and Properties

        private IRepository<Log> _repository { get; }

        #endregion


        #region Constructor

        public LogService(IRepository<Log> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new Log as a new record in the Log table
        /// </summary>
        /// <param name="entity">The Log that gets added to the table in the database</param>
        /// <returns>A taks of Log</returns>
        public async Task<Log> CreateAsync(Log entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a Log from the Log table
        /// </summary>
        /// <param name="id">The id of the Log that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the Log table
        /// </summary>
        /// <param name="entities">The list of Logs that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<Log> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table Log by its id
        /// </summary>
        /// <param name="id">The id of the Log that gets returned</param>
        /// <returns>The (task of) Log that has a matching id with the given id</returns>
        public async Task<Log> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new Log();
            }
        }

        /// <summary>
        /// Returns a list containing all records of Log table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in Log table with IsRemoved of false</returns>
        public async Task<List<Log>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Log>();

            return entityList;
        }

        /// <summary>
        /// Updates a Log
        /// </summary>
        /// <param name="entity">The modified Log</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(Log entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
