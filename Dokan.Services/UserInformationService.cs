using Dokan.Core.DataAccess;
using Dokan.Domain.UsersAndRoles;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class UserInformationService : IUserInformationService
    {
        #region Fields and Properties

        private IRepository<UserInformation> _repository { get; }

        #endregion


        #region Constructor

        public UserInformationService(IRepository<UserInformation> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new UserInformation as a new record in the UserInformation table
        /// </summary>
        /// <param name="entity">The UserInformation that gets added to the table in the database</param>
        /// <returns>A taks of UserInformation</returns>
        public async Task<UserInformation> CreateAsync(UserInformation entity)
        {
            try
            {
                return await _repository.CreateAsync(entity);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes a UserInformation from the UserInformation table
        /// </summary>
        /// <param name="id">The id of the UserInformation that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the UserInformation table
        /// </summary>
        /// <param name="entities">The list of UserInformations that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<UserInformation> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table UserInformation by its id
        /// </summary>
        /// <param name="id">The id of the UserInformation that gets returned</param>
        /// <returns>The (task of) UserInformation that has a matching id with the given id</returns>
        public async Task<UserInformation> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new UserInformation();
            }
        }

        /// <summary>
        /// Returns a list containing all records of UserInformation table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in UserInformation table with IsRemoved of false</returns>
        public async Task<List<UserInformation>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<UserInformation>();

            return entityList;
        }

        /// <summary>
        /// Updates a UserInformation
        /// </summary>
        /// <param name="entity">The modified UserInformation</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(UserInformation entity)
        {
            await _repository.UpdateAsync(entity);
        }

        #endregion
    }
}
