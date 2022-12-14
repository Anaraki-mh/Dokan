using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class TestimonialService : ITestimonialService
    {
        #region Fields and Properties

        private IRepository<Testimonial> _repository { get; }

        #endregion


        #region Constructor

        public TestimonialService(IRepository<Testimonial> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new Testimonial as a new record in the Testimonial table
        /// </summary>
        /// <param name="entity">The Testimonial that gets added to the table in the database</param>
        /// <returns>A taks of Testimonial</returns>
        public async Task<Testimonial> CreateAsync(Testimonial entity)
        {
            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a Testimonial from the Testimonial table
        /// </summary>
        /// <param name="id">The id of the Testimonial that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the Testimonial table
        /// </summary>
        /// <param name="entities">The list of Testimonials that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<Testimonial> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table Testimonial by its id
        /// </summary>
        /// <param name="id">The id of the Testimonial that gets returned</param>
        /// <returns>The (task of) Testimonial that has a matching id with the given id</returns>
        public async Task<Testimonial> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new Testimonial();
            }
        }

        /// <summary>
        /// Returns a list containing all records of Testimonial table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in Testimonial table with IsRemoved of false</returns>
        public async Task<List<Testimonial>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Testimonial>();

            return entityList
                .Where(x => !x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Returns a list containing all records of Testimonial table where the IsRemoved property is true
        /// </summary>
        /// <returns>A (task of) list of all the entities in Testimonial table with IsRemoved of true</returns>
        public async Task<List<Testimonial>> ListOfRemovedAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<Testimonial>();

            return entityList
                .Where(x => x.IsRemoved)
                .ToList();
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Testimonial to true
        /// </summary>
        /// <param name="id">The id of the Testimonial which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RemoveAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Testimonial(); ;
            entity.IsRemoved = true;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Changes the value of IsRemoved property of a Testimonial to false
        /// </summary>
        /// <param name="id">The id of the Testimonial which gets its IsRemoved property updated</param>
        /// <returns>A task</returns>
        public async Task RestoreAsync(int id)
        {
            var entity = await FindByIdAsync(id) ?? new Testimonial(); ;
            entity.IsRemoved = false;
            await UpdateAsync(entity);
        }

        /// <summary>
        /// Updates a Testimonial
        /// </summary>
        /// <param name="entity">The modified Testimonial</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(Testimonial entity)
        {
            await _repository.UpdateAsync(entity);
        }


        #endregion
    }
}
