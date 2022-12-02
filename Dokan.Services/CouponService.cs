using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class CouponService : ICouponService
    {
        #region Fields and Properties

        private IRepository<Coupon> _repository { get; }

        #endregion


        #region Constructor

        public CouponService(IRepository<Coupon> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new Coupon as a new record in the Coupon table
        /// </summary>
        /// <param name="entity">The Coupon that gets added to the table in the database</param>
        /// <returns>A taks of Coupon</returns>
        public async Task<Coupon> CreateAsync(Coupon entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a Coupon from the Coupon table
        /// </summary>
        /// <param name="id">The id of the Coupon that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the Coupon table
        /// </summary>
        /// <param name="entities">The list of Coupons that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<Coupon> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table Coupon by its id
        /// </summary>
        /// <param name="id">The id of the Coupon that gets returned</param>
        /// <returns>The (task of) Coupon that has a matching id with the given id</returns>
        public async Task<Coupon> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new Coupon();
            }
        }

        /// <summary>
        /// Returns a list containing all records of Coupon table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in Coupon table with IsRemoved of false</returns>
        public async Task<List<Coupon>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Coupon>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of Coupon table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in Coupon table with IsRemoved of true</returns>
        public async Task<List<Coupon>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Coupon>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Coupon to true
        /// </summary>
        /// <param name="id">The id of the Coupon which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Coupon(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Coupon to false
        /// </summary>
        /// <param name="id">The id of the Coupon which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Coupon(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a Coupon
        /// </summary>
        /// <param name="entity">The modified Coupon</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(Coupon entity)
        {
            await _repository.UpdateAsync(entity);
        }

        /// <summary>
        /// Searches for the searchString in the Coupon's Code
        /// </summary>
        /// <param name="searchString">A string that gets searched for in the Coupon table</param>
        /// <returns>A valid Coupon that is active and hasnt expired</returns>
        public async Task<Coupon> SearchAsync(string searchString)
        {
            var list = await ListAsync();
            var entity = new Coupon();

            if (!string.IsNullOrEmpty(searchString) && list != null)
            {
                entity = list
                    .FirstOrDefault(x => x.Code.Contains(searchString) && x.IsActive == true && x.ExpiryDateTime > DateTime.UtcNow);
            }

            return entity;
        }

        #endregion
    }
}
