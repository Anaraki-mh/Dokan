using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class MenuService : IMenuService
    {
        #region Fields and Properties

        private IRepository<Menu> _repository { get; }

        #endregion


        #region Constructor

        public MenuService(IRepository<Menu> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new Menu as a new record in the Menu table
        /// </summary>
        /// <param name="entity">The Menu that gets added to the table in the database</param>
        /// <returns>A taks of Menu</returns>
        public async Task<Menu> CreateAsync(Menu entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a Menu from the Menu table
        /// </summary>
        /// <param name="id">The id of the Menu that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the Menu table
        /// </summary>
        /// <param name="entities">The list of Menus that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<Menu> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table Menu by its id
        /// </summary>
        /// <param name="id">The id of the Menu that gets returned</param>
        /// <returns>The (task of) Menu that has a matching id with the given id</returns>
        public async Task<Menu> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new Menu();
            }
        }

        /// <summary>
        /// Returns a list containing all records of Menu table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in Menu table with IsRemoved of false</returns>
        public async Task<List<Menu>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Menu>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of Menu table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in Menu table with IsRemoved of true</returns>
        public async Task<List<Menu>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Menu>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Menu to true
        /// </summary>
        /// <param name="id">The id of the Menu which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Menu(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Menu to false
        /// </summary>
        /// <param name="id">The id of the Menu which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Menu(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a Menu
        /// </summary>
        /// <param name="entity">The modified Menu</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(Menu entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
