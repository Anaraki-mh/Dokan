using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class CarouselService : ICarouselService
    {
        #region Fields and Properties

        private IRepository<Carousel> _repository { get; }

        #endregion


        #region Constructor

        public CarouselService(IRepository<Carousel> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new Carousel as a new record in the Carousel table
        /// </summary>
        /// <param name="entity">The Carousel that gets added to the table in the database</param>
        /// <returns>A taks of Carousel</returns>
        public async Task<Carousel> CreateAsync(Carousel entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a Carousel from the Carousel table
        /// </summary>
        /// <param name="id">The id of the Carousel that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the Carousel table
        /// </summary>
        /// <param name="entities">The list of Carousels that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<Carousel> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table Carousel by its id
        /// </summary>
        /// <param name="id">The id of the Carousel that gets returned</param>
        /// <returns>The (task of) Carousel that has a matching id with the given id</returns>
        public async Task<Carousel> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new Carousel();
            }
        }

        /// <summary>
        /// Returns a list containing all records of Carousel table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in Carousel table with IsRemoved of false</returns>
        public async Task<List<Carousel>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Carousel>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of Carousel table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in Carousel table with IsRemoved of true</returns>
        public async Task<List<Carousel>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Carousel>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Carousel to true
        /// </summary>
        /// <param name="id">The id of the Carousel which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Carousel(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Carousel to false
        /// </summary>
        /// <param name="id">The id of the Carousel which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task AddBackAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Carousel(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a Carousel
        /// </summary>
        /// <param name="entity">The modified Carousel</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(Carousel entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
