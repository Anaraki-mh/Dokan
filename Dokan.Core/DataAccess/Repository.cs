using Dokan.Core.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Core.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Fields and Properties

        private DokanContext _context { get; }

        #endregion


        #region Constructor

        public Repository(DokanContext context)
        {
            _context = context;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Adds a new row to the table that matches the type of the entity.
        /// </summary>
        /// <param name="entity">The object that gets saved in the database as a row in the corresponding table.</param>
        /// <returns>The taks of adding the entity</returns>
        public async Task<T> CreateAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Deletes the entity from the correspinding table in the database
        /// </summary>
        /// <param name="entity">The task of the entity that gets removed from the entity</param>
        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Gets a list of all the entities in a table in the database
        /// </summary>
        /// <returns>A task of the list of all the saved entities in a table of the database</returns>
        public async Task<List<T>> ListAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Modifies an existing record 
        /// </summary>
        /// <param name="entity">The task of the modified version of an exisiting record in a table in the database</param>
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().AddOrUpdate(entity);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
