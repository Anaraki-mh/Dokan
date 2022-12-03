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
    public class UserService : IUserService
    {
        #region Fields and Properties

        private IRepository<User> _repository { get; }

        #endregion


        #region Constructor

        public UserService(IRepository<User> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new User as a new record in the User table
        /// </summary>
        /// <param name="entity">The User that gets added to the table in the database</param>
        /// <returns>A taks of User</returns>
        public async Task<User> CreateAsync(User entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a User from the User table
        /// </summary>
        /// <param name="id">The id of the User that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the User table
        /// </summary>
        /// <param name="entities">The list of Users that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<User> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table User by its id
        /// </summary>
        /// <param name="id">The id of the User that gets returned</param>
        /// <returns>The (task of) User that has a matching id with the given id</returns>
        public async Task<User> FindByIdAsync(string id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new User();
            }
        }

        /// <summary>
        /// Returns a list containing all records of User table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in User table with IsRemoved of false</returns>
        public async Task<List<User>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<User>();

            return entityList;
        }

        /// <summary>
        /// Updates a User
        /// </summary>
        /// <param name="entity">The modified User</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(User entity)
        {
            await _repository.UpdateAsync(entity);
        }


        /// <summary>
        /// Searches for the searchString in the User's Title and Content
        /// </summary>
        /// <param name="searchString">A string that gets searched for in the User table</param>
        /// <returns>A list of entities that contain the searchString in their Title or Content</returns>
        public async Task<List<User>> SearchAsync(string searchString)
        {
            var list = await ListAsync();
            searchString = searchString.ToLower();

            if (!string.IsNullOrEmpty(searchString) && list != null)
            {
                list = list
                    .Where(x => x.UserName.ToLower().Contains(searchString)
                    || x.UserInformation.FirstName.ToLower().Contains(searchString)
                    || x.UserInformation.LastName.ToLower().Contains(searchString)
                    || x.UserInformation.PhoneNumber.ToLower().Contains(searchString)
                    || x.UserInformation.Address.ToLower().Contains(searchString)
                    || x.Email.ToLower().Contains(searchString))
                    .ToList();
            }
            else
                list = new List<User>();

            return list;
        }

        #endregion
    }
}
